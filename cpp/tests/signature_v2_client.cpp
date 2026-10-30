#include <alibabacloud/Openapi.hpp>
#include <darabonba/Runtime.hpp>
#include <iostream>
#include <map>
#include <string>

using namespace AlibabaCloud::OpenApi;
using namespace AlibabaCloud::OpenApi::Utils::Models;

int main(int argc, char **argv) {
  if (argc != 4) return 2;
  const std::string style = argv[2];
  const std::string scenario = argv[3];
  Config config;
  config.setEndpoint("example.com").setProtocol("http").setHttpProxy("http://" + std::string(argv[1]));
  if (scenario == "bearer") config.setBearerToken("token");
  else config.setAccessKeyId("ak").setAccessKeySecret("secret");
  if (scenario == "override") {
    GlobalParameters global;
    global.setHeaders({{"x-acs-signature-method", "HMAC-SHA256"}});
    config.setGlobalParameters(global);
  }
  OpenApiRequest request;
  if (scenario == "override") request.setHeaders({{"X-Acs-Signature-Method", "HMAC-SHA1"}});
  if (scenario == "conflict") request.setHeaders({{"X-Acs-Signature-Method", "HMAC-SHA1"}, {"x-acs-signature-method", "HMAC-SHA256"}});
  if (scenario == "invalid") request.setHeaders({{"X-Acs-Signature-Method", "invalid"}});
  Darabonba::RuntimeOptions runtime;
  runtime.setAutoretry(false).setMaxAttempts(1).setReadTimeout(5000).setConnectTimeout(5000);
  try {
    Client client(config);
    const std::string auth = scenario == "anonymous" ? "Anonymous" : "AK";
    if (style == "RPC") client.doRPCRequest("TestAction", "2022-06-01", "HTTP", "GET", auth, "json", request, runtime);
    else client.doROARequest("TestAction", "2022-06-01", "HTTP", "GET", auth, "/test", "json", request, runtime);
  } catch (const std::exception &error) {
    std::cerr << error.what() << std::endl;
    return 1;
  }
  return 0;
}
