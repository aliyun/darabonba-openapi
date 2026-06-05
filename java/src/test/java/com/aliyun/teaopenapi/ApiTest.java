package com.aliyun.teaopenapi;

import com.aliyun.teaopenapi.models.Config;
import com.aliyun.teaopenapi.models.OpenApiRequest;
import com.aliyun.teaopenapi.models.Params;
import com.aliyun.teautil.models.RuntimeOptions;
import com.github.tomakehurst.wiremock.core.WireMockConfiguration;
import com.github.tomakehurst.wiremock.junit.WireMockRule;
import org.junit.Assert;
import org.junit.Rule;
import org.junit.Test;

import java.lang.management.ManagementFactory;
import java.lang.management.ThreadInfo;
import java.lang.management.ThreadMXBean;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicBoolean;
import java.util.concurrent.atomic.AtomicInteger;

import static com.github.tomakehurst.wiremock.client.WireMock.aResponse;
import static com.github.tomakehurst.wiremock.client.WireMock.anyUrl;
import static com.github.tomakehurst.wiremock.client.WireMock.post;
import static com.github.tomakehurst.wiremock.client.WireMock.stubFor;

/**
 * Real end-to-end tests for {@code Config.callTimeout}.
 *
 * <p>These tests issue actual {@code callApi} requests against a local WireMock server whose
 * response is delayed far beyond the configured {@code callTimeout}, so the timeout is the only
 * thing that can end the call. A concurrency test additionally verifies that {@code callTimeout}
 * does not allocate a timer thread per call (OkHttp/Okio uses a single shared watchdog), i.e. it
 * does not blow up thread-pool allocation under load.
 */
public class ApiTest {

    // High container thread count so WireMock can hold hundreds of slow (delayed) requests at once.
    @Rule
    public WireMockRule wireMock = new WireMockRule(
            WireMockConfiguration.options().dynamicPort().containerThreads(700));

    private Client createClient(int port, Integer callTimeout) throws Exception {
        Config config = new Config()
                .setAccessKeyId("ak")
                .setAccessKeySecret("secret")
                .setEndpoint("localhost:" + port)
                .setProtocol("HTTP")
                // Keep per-phase timeouts large so that callTimeout is the limiter, not readTimeout.
                .setConnectTimeout(10000)
                .setReadTimeout(10000)
                .setCallTimeout(callTimeout);
        return new Client(config);
    }

    private Params createApiInfo() throws Exception {
        return new Params()
                .setAction("DescribeRegions")
                .setVersion("2014-05-26")
                .setProtocol("HTTPS")
                .setMethod("POST")
                .setAuthType("AK")
                .setStyle("RPC")
                .setPathname("/")
                .setReqBodyType("json")
                .setBodyType("json");
    }

    private static boolean isTimeoutLike(Throwable e) {
        String msg = e.getMessage() == null ? "" : e.getMessage().toLowerCase();
        return msg.contains("timeout")
                || msg.contains("canceled")
                || msg.contains("closed")
                || msg.contains("reset");
    }

    private static int countThreads(String lowerNeedle) {
        ThreadMXBean bean = ManagementFactory.getThreadMXBean();
        long[] ids = bean.getAllThreadIds();
        ThreadInfo[] infos = bean.getThreadInfo(ids, 0);
        int count = 0;
        for (ThreadInfo info : infos) {
            if (info == null) {
                continue;
            }
            String name = info.getThreadName();
            if (name != null && name.toLowerCase().contains(lowerNeedle)) {
                count++;
            }
        }
        return count;
    }

    private static int countHttpThreads() {
        return countThreads("okhttp") + countThreads("okio");
    }

    @Test
    public void testCallTimeoutTriggersOnRealCall() throws Exception {
        // Response is delayed 3s; callTimeout is 500ms, so the call must abort around 500ms.
        stubFor(post(anyUrl()).willReturn(aResponse()
                .withStatus(200)
                .withFixedDelay(3000)
                .withBody("{\"RequestId\":\"test\"}")));

        Client client = createClient(wireMock.port(), 500);
        Params params = createApiInfo();
        OpenApiRequest request = new OpenApiRequest();
        RuntimeOptions runtime = new RuntimeOptions(); // autoretry defaults off -> single attempt

        long start = System.currentTimeMillis();
        try {
            client.callApi(params, request, runtime);
            Assert.fail("expected callTimeout to abort the request");
        } catch (Exception e) {
            long cost = System.currentTimeMillis() - start;
            Assert.assertTrue("should fail with a timeout-like error, got: " + e.getMessage(),
                    isTimeoutLike(e));
            // Must be near callTimeout (500ms), well before the 3000ms response delay.
            Assert.assertTrue("call should abort near callTimeout, cost=" + cost + "ms", cost < 2500);
        }
    }

    @Test
    public void testCallTimeoutPriorityOverReadTimeout() throws Exception {
        // readTimeout=10s would NOT trip within the 3s delay, but callTimeout=800ms will.
        stubFor(post(anyUrl()).willReturn(aResponse()
                .withStatus(200)
                .withFixedDelay(3000)
                .withBody("{\"RequestId\":\"test\"}")));

        Client client = createClient(wireMock.port(), 800);
        Params params = createApiInfo();
        OpenApiRequest request = new OpenApiRequest();
        RuntimeOptions runtime = new RuntimeOptions();

        long start = System.currentTimeMillis();
        try {
            client.callApi(params, request, runtime);
            Assert.fail("expected callTimeout to abort the request");
        } catch (Exception e) {
            long cost = System.currentTimeMillis() - start;
            Assert.assertTrue("should fail with a timeout-like error, got: " + e.getMessage(),
                    isTimeoutLike(e));
            Assert.assertTrue("callTimeout should win over readTimeout, cost=" + cost + "ms", cost < 2500);
        }
    }

    /**
     * High-concurrency timing accuracy test. For each configured callTimeout, fires a burst of
     * {@code CONCURRENCY} requests <em>simultaneously</em> (released together via a start latch) and
     * collects the abort latency of every single call. Asserts, across all 500 samples per value:
     * <ul>
     *   <li>none fire earlier than the configured timeout (minus small clock jitter);</li>
     *   <li>p99 stays within a bounded overhead;</li>
     *   <li>the slowest call still aborts well before the server would have responded
     *       (i.e. it really is callTimeout, not the server delay).</li>
     * </ul>
     * Prints a full latency-distribution report (min/p50/p90/p99/max) to stdout.
     */
    @Test
    public void testCallTimeoutTimingUnderHighConcurrency() throws Exception {
        final int serverDelayMs = 6000; // far beyond every configured callTimeout below
        stubFor(post(anyUrl()).willReturn(aResponse()
                .withStatus(200)
                .withFixedDelay(serverDelayMs)
                .withBody("{\"RequestId\":\"test\"}")));

        final int concurrency = 500;
        int[] configuredTimeouts = {500, 1000};
        long earlyJitterMs = 150;   // must not fire before configured - jitter
        long p99OverheadMs = 2000;  // 99% of calls within configured + this
        long maxOverheadMs = 4000;  // absolute cap, still < serverDelay -> proves it is callTimeout

        StringBuilder report = new StringBuilder();
        report.append("\n============================ CallTimeout High-Concurrency Timing Report ============================\n");
        report.append(String.format("%-14s %-8s %-9s %-9s %-9s %-9s %-9s %-9s %-10s %-7s%n",
                "configured", "samples", "ok", "min", "p50", "p90", "p99", "max", "avgOvhd", "verdict"));
        report.append("---------------------------------------------------------------------------------------------------\n");

        for (int configured : configuredTimeouts) {
            long[] latencies = runConcurrentBurst(configured, concurrency);

            long[] sorted = latencies.clone();
            Arrays.sort(sorted);
            long min = sorted[0];
            long max = sorted[sorted.length - 1];
            long p50 = percentile(sorted, 50);
            long p90 = percentile(sorted, 90);
            long p99 = percentile(sorted, 99);
            long sum = 0;
            for (long l : sorted) {
                sum += l;
            }
            long avg = sum / sorted.length;
            long avgOverhead = avg - configured;

            // Never earlier than configured (timer must not fire prematurely) - checked on EVERY sample via min.
            Assert.assertTrue("callTimeout=" + configured + "ms fired too early under load: min=" + min + "ms",
                    min >= configured - earlyJitterMs);
            // 99% within a tight overhead window.
            Assert.assertTrue("callTimeout=" + configured + "ms p99 too high under load: p99=" + p99 + "ms",
                    p99 <= configured + p99OverheadMs);
            // Even the worst sample aborts well before the server delay -> it is callTimeout, not the response.
            Assert.assertTrue("callTimeout=" + configured + "ms max too high under load: max=" + max + "ms",
                    max <= configured + maxOverheadMs);
            Assert.assertTrue("max must stay below server delay to prove callTimeout caused the abort",
                    max < serverDelayMs);

            report.append(String.format("%-14d %-8d %-9d %-9d %-9d %-9d %-9d %-9d %-10d %-7s%n",
                    configured, concurrency, concurrency, min, p50, p90, p99, max, avgOverhead, "OK"));
        }
        report.append("===================================================================================================\n");
        report.append("Window rule: configured - ").append(earlyJitterMs)
                .append("ms <= abort <= configured + ").append(maxOverheadMs)
                .append("ms (server delay = ").append(serverDelayMs).append("ms)\n");
        System.out.println(report);
    }

    /**
     * Fires {@code concurrency} requests at once (all released via a single start latch) against a
     * client configured with the given callTimeout, returning each call's abort latency in ms.
     * Asserts that every call was actually ended by a timeout (no success, no other failure).
     */
    private long[] runConcurrentBurst(int configuredTimeout, final int concurrency) throws Exception {
        final Client client = createClient(wireMock.port(), configuredTimeout);
        final Params params = createApiInfo();
        final OpenApiRequest request = new OpenApiRequest();

        // Warm up shared infra (Okio watchdog, TaskRunner) so it is not attributed to the burst.
        try {
            client.callApi(params, request, new RuntimeOptions());
            Assert.fail("warm-up call should time out");
        } catch (Exception ignore) {
            // expected
        }

        final long[] latencies = new long[concurrency];
        final AtomicInteger timeoutCount = new AtomicInteger(0);
        final AtomicInteger unexpectedSuccess = new AtomicInteger(0);
        final AtomicInteger otherFailure = new AtomicInteger(0);
        final CountDownLatch ready = new CountDownLatch(concurrency);
        final CountDownLatch startGate = new CountDownLatch(1);
        final CountDownLatch done = new CountDownLatch(concurrency);

        ExecutorService pool = Executors.newFixedThreadPool(concurrency);
        for (int i = 0; i < concurrency; i++) {
            final int idx = i;
            pool.submit(new Runnable() {
                @Override
                public void run() {
                    ready.countDown();
                    try {
                        startGate.await();
                    } catch (InterruptedException e) {
                        Thread.currentThread().interrupt();
                        return;
                    }
                    long t0 = System.nanoTime();
                    try {
                        client.callApi(params, request, new RuntimeOptions());
                        unexpectedSuccess.incrementAndGet();
                    } catch (Exception e) {
                        latencies[idx] = (System.nanoTime() - t0) / 1_000_000L;
                        if (isTimeoutLike(e)) {
                            timeoutCount.incrementAndGet();
                        } else {
                            otherFailure.incrementAndGet();
                        }
                    } finally {
                        done.countDown();
                    }
                }
            });
        }

        Assert.assertTrue("workers did not become ready", ready.await(60, TimeUnit.SECONDS));
        startGate.countDown(); // release the whole burst simultaneously
        Assert.assertTrue("burst did not finish in time", done.await(120, TimeUnit.SECONDS));
        pool.shutdownNow();

        Assert.assertEquals("callTimeout=" + configuredTimeout + "ms: a delayed response must never succeed",
                0, unexpectedSuccess.get());
        Assert.assertEquals("callTimeout=" + configuredTimeout + "ms: non-timeout failures occurred",
                0, otherFailure.get());
        Assert.assertEquals("callTimeout=" + configuredTimeout + "ms: all calls should hit callTimeout",
                concurrency, timeoutCount.get());

        // Drain lingering server-side delayed responses so the next burst starts clean.
        Thread.sleep(serverSettleMs());
        return latencies;
    }

    private static long serverSettleMs() {
        return 6500L;
    }

    private static long percentile(long[] sortedAsc, double p) {
        if (sortedAsc.length == 0) {
            return 0;
        }
        int idx = (int) Math.ceil((p / 100.0) * sortedAsc.length) - 1;
        if (idx < 0) {
            idx = 0;
        }
        if (idx >= sortedAsc.length) {
            idx = sortedAsc.length - 1;
        }
        return sortedAsc[idx];
    }

    /**
     * Fire many concurrent requests that all hit callTimeout and assert that the timeout
     * implementation does NOT spawn a timer thread per call: OkHttp/Okio relies on a single shared
     * watchdog, so the number of HTTP-related (okhttp/okio) threads must stay a small constant
     * regardless of how many calls time out concurrently.
     */
    @Test
    public void testCallTimeoutConcurrencyDoesNotLeakThreads() throws Exception {
        stubFor(post(anyUrl()).willReturn(aResponse()
                .withStatus(200)
                .withFixedDelay(2000)
                .withBody("{\"RequestId\":\"test\"}")));

        final Client client = createClient(wireMock.port(), 500);
        final Params params = createApiInfo();
        final OpenApiRequest request = new OpenApiRequest();

        // Warm up: initialize shared infra (Okio watchdog, connection pool, TaskRunner).
        try {
            client.callApi(params, request, new RuntimeOptions());
            Assert.fail("warm-up call should time out");
        } catch (Exception expected) {
            Assert.assertTrue(isTimeoutLike(expected));
        }

        final int concurrency = 20;
        final int totalTasks = 60;

        final int httpThreadsBaseline = countHttpThreads();

        // Background monitor records the peak okhttp/okio thread usage and peak okio-watchdog count.
        final AtomicBoolean monitoring = new AtomicBoolean(true);
        final AtomicInteger peakHttpThreads = new AtomicInteger(httpThreadsBaseline);
        final AtomicInteger peakWatchdogThreads = new AtomicInteger(0);
        Thread monitor = new Thread(new Runnable() {
            @Override
            public void run() {
                while (monitoring.get()) {
                    bumpMax(peakHttpThreads, countHttpThreads());
                    bumpMax(peakWatchdogThreads, countThreads("okio watchdog"));
                    try {
                        Thread.sleep(15);
                    } catch (InterruptedException e) {
                        return;
                    }
                }
            }
        }, "callTimeout-thread-monitor");
        monitor.setDaemon(true);
        monitor.start();

        final AtomicInteger timeoutCount = new AtomicInteger(0);
        final AtomicInteger unexpectedSuccess = new AtomicInteger(0);
        final AtomicInteger otherFailure = new AtomicInteger(0);

        ExecutorService pool = Executors.newFixedThreadPool(concurrency);
        List<Future<?>> futures = new ArrayList<>();
        for (int i = 0; i < totalTasks; i++) {
            futures.add(pool.submit(new Runnable() {
                @Override
                public void run() {
                    try {
                        client.callApi(params, request, new RuntimeOptions());
                        unexpectedSuccess.incrementAndGet();
                    } catch (Exception e) {
                        if (isTimeoutLike(e)) {
                            timeoutCount.incrementAndGet();
                        } else {
                            otherFailure.incrementAndGet();
                        }
                    }
                }
            }));
        }
        for (Future<?> f : futures) {
            f.get(60, TimeUnit.SECONDS);
        }
        pool.shutdown();
        Assert.assertTrue("worker pool did not terminate", pool.awaitTermination(30, TimeUnit.SECONDS));

        monitoring.set(false);
        monitor.join(5000);

        // Let shared daemon threads (watchdog / connection-pool cleanup) settle back down.
        Thread.sleep(2000);
        int httpThreadsAfter = countHttpThreads();

        // 1) Every concurrent call must actually be ended by callTimeout.
        Assert.assertEquals("a delayed response must never succeed", 0, unexpectedSuccess.get());
        Assert.assertEquals("non-timeout failures occurred", 0, otherFailure.get());
        Assert.assertEquals("all calls should hit callTimeout", totalTasks, timeoutCount.get());

        // 2) callTimeout's timer is a single shared Okio watchdog, never one-per-call.
        Assert.assertTrue("okio watchdog must be a single shared thread, peak=" + peakWatchdogThreads.get(),
                peakWatchdogThreads.get() <= 1);

        // 3) HTTP-related threads must stay a small constant, NOT scale with the number of
        //    concurrent timed-out calls (which would indicate per-call thread allocation).
        int peakDelta = peakHttpThreads.get() - httpThreadsBaseline;
        Assert.assertTrue("okhttp/okio threads grew with load (per-call allocation?): baseline="
                        + httpThreadsBaseline + ", peak=" + peakHttpThreads.get()
                        + ", concurrency=" + concurrency + ", totalTasks=" + totalTasks,
                peakDelta <= concurrency);

        // 4) Synchronous calls must not create async dispatcher threads.
        Assert.assertEquals("no async dispatcher threads expected for synchronous calls",
                0, countThreads("okhttp dispatcher"));

        // 5) After the load settles, threads return close to baseline (no leak).
        Assert.assertTrue("http threads not released after load: baseline=" + httpThreadsBaseline
                        + ", after=" + httpThreadsAfter,
                httpThreadsAfter - httpThreadsBaseline <= 5);
    }

    private static void bumpMax(AtomicInteger holder, int candidate) {
        int prev;
        do {
            prev = holder.get();
            if (candidate <= prev) {
                return;
            }
        } while (!holder.compareAndSet(prev, candidate));
    }
}
