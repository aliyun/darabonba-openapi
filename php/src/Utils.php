<?php

// This file is auto-generated, don't edit it. Thanks.
 
namespace Darabonba\OpenApi;
use AlibabaCloud\Dara\Model;
use RuntimeException;
use AlibabaCloud\Dara\Request;
use AlibabaCloud\Dara\Util\BytesUtil;
use Psr\Http\Message\StreamInterface;
/**
 * @remarks
 * This is for OpenApi Util
 */
class Utils {

  private static $defaultUserAgent = '';
  /**
   * @remarks
   * Convert all params of body other than type of readable into content
   * 
   * @param Model $body
   * @param Model $content
   * @return void
   */
  static public function convert($body, $content)
  {
    $map = $body->toMap();
    $map = self::exceptStream($map);
    $newContent = $content::fromMap($map);
    $class = new \ReflectionClass($newContent);
    foreach ($class->getProperties(\ReflectionProperty::IS_PUBLIC) as $property) {
      $name = $property->getName();
      if (!$property->isStatic()) {
        $content->{$name} = $property->getValue($newContent);
      }
    }
  }

  private static function exceptStream($map)
  {
    if ($map instanceof StreamInterface) {
      return null;
    } elseif (\is_array($map)) {
      $data = [];
      foreach ($map as $k => $v) {
        if (null !== $v) {
          $item = self::exceptStream($v);
          if (null !== $item) {
            $data[$k] = $item;
          }
        } else {
          $data[$k] = $v;
        }
      }
      return $data;
    }
    return $map;
  }

  /**
   * @remarks
   * If endpointType is internal, use internal endpoint
   * If serverUse is true and endpointType is accelerate, use accelerate endpoint
   * Default return endpoint
   * 
   * @param string $endpoint The endpoint URL
   * @param boolean $useAccelerate Whether use accelerate endpoint
   * @param string $endpointType Value must be internal or accelerate
   * @return string The final endpoint
   */
  static public function getEndpoint($endpoint, $useAccelerate, $endpointType)
  {
    if ('internal' == $endpointType) {
      $tmp      = explode('.', $endpoint);
      $tmp[0]   .= '-internal';
      $endpoint = implode('.', $tmp);
    }
    if ($useAccelerate && 'accelerate' == $endpointType) {
      return 'oss-accelerate.aliyuncs.com';
    }

    return $endpoint;
  }

  /**
   * @remarks
   * Get throttling param
   * 
   * @param string[] $headers The response headers
   * @return int Time left
   */
  public static function getThrottlingTimeLeft(array $headers) {
    $rateLimitForUserApi = isset($headers["x-ratelimit-user-api"]) ? $headers["x-ratelimit-user-api"] : null;
    $rateLimitForUser = isset($headers["x-ratelimit-user"]) ? $headers["x-ratelimit-user"] : null;

    $timeLeftForUserApi = self::getTimeLeft($rateLimitForUserApi);
    $timeLeftForUser = self::getTimeLeft($rateLimitForUser);
    
    $maxValue = max($timeLeftForUserApi, $timeLeftForUser);
    return $maxValue !== null ? $maxValue : 0;
  }


private static function getTimeLeft($rateLimit) {
  if ($rateLimit) {
    $pairs = explode(',', $rateLimit);
    foreach ($pairs as $pair) {
      $kv = explode(':', $pair);
      if (count($kv) === 2) {
        $key = trim($kv[0]);
        $value = trim($kv[1]);
        if ($key === 'TimeLeft') {
          $timeLeftValue = intval($value); 
          if ($timeLeftValue === 0 && $value !== "0") { // 确认不是 "0"
            return null; 
          }
          return $timeLeftValue;
        }
      }
    }
  }
  return null;
}

  /**
   * @remarks
   * Hash the raw data with signatureAlgorithm
   * 
   * @param int[] $raw Hashing data
   * @param string $signatureAlgorithm The autograph method
   * @return string|array Hashed bytes
   */
  static public function hash($raw, $signatureAlgorithm)
  {
    $str = BytesUtil::toString($raw);

    switch ($signatureAlgorithm) {
        case 'ACS3-HMAC-SHA256':
        case 'ACS3-RSA-SHA256':
            $res = hash('sha256', $str, true);
            return $res;
        case 'ACS3-HMAC-SM3':
            $res = self::sm3($str);
            return hex2bin($res);
    }

    return [];
  }

  /**
   * @remarks
   * Generate a nonce string
   * @returns the nonce string
   * @return string
   */
  static public function getNonce()
  {
    return md5(uniqid() . uniqid(md5(microtime(true)), true));
  }

  /**
   * @remarks
   * Get the string to be signed according to request
   * 
   * @param Request $request Which contains signed messages
   * @return string The signed string
   */
  static public function getStringToSign($request)
  {
    $pathname = $request->pathname ?: '';
    $query    = $request->query ?: [];

    $accept      = isset($request->headers['accept']) ? $request->headers['accept'] : '';
    $contentMD5  = isset($request->headers['content-md5']) ? $request->headers['content-md5'] : '';
    $contentType = isset($request->headers['content-type']) ? $request->headers['content-type'] : '';
    $date        = isset($request->headers['date']) ? $request->headers['date'] : '';

    $result = $request->method . "\n" .
      $accept . "\n" .
      $contentMD5 . "\n" .
      $contentType . "\n" .
      $date . "\n";

    $canonicalizedHeaders  = self::getCanonicalizedHeaders($request->headers);
    $canonicalizedResource = self::getCanonicalizedResource($pathname, $query);

    return $result . $canonicalizedHeaders . $canonicalizedResource;
  }

  private static function getCanonicalizedHeaders($headers, $prefix = 'x-acs-')
  {
    ksort($headers);
    $str = '';
    foreach ($headers as $k => $v) {
      if (0 === strpos(strtolower($k), $prefix)) {
        $str .= $k . ':' . trim(self::filter($v)) . "\n";
      }
    }

    return $str;
  }

  private static function getCanonicalizedResource($pathname, $query)
  {
    if (0 === \count($query)) {
      return $pathname;
    }
    ksort($query);
    $tmp = [];
    foreach ($query as $k => $v) {
      if (!empty($v)) {
        $tmp[] = $k . '=' . $v;
      } else {
        $tmp[] = $k;
      }
    }

    return $pathname . '?' . implode('&', $tmp);
  }

  /**
   * @remarks
   * Get signature according to stringToSign, secret
   * 
   * @param string $stringToSign The signed string
   * @param string $secret Accesskey secret
   * @return string The signature
   */
  static public function getROASignature($stringToSign, $secret)
  {
    return base64_encode(hash_hmac('sha1', $stringToSign, $secret, true));
  }

  /**
   * @remarks
   * Parse filter into a form string
   * 
   * @param mixed[] $filter Object
   * @return string The string
   */
  static public function toForm($filter)
  {
    $query = $filter;
      if (null === $query) {
          return '';
      }
      if ($query instanceof Model) {
          $query = $query->toMap();
      }
      $tmp = [];
      foreach ($query as $k => $v) {
          if (0 !== strpos($k, '_')) {
              $tmp[$k] = $v;
          }
      }
      $res = self::flatten($tmp);
      ksort($res);

      return http_build_query($res);
  }

  /**
   * @remarks
   * Get timestamp
   * @returns the timestamp string
   * @return string
   */
  static public function getTimestamp()
  {
    return gmdate('Y-m-d\\TH:i:s\\Z');
  }

  /**
   * @remarks
   * Get UTC string
   * @returns the UTC string
   * @return string
   */
  static public function getDateUTCString()
  {
    return gmdate('D, d M Y H:i:s T');
  }

  /**
   * @remarks
   * Parse filter into a object which's type is map[string]string
   * 
   * @param mixed[] $filter Query param
   * @return string[] The object
   */
  static public function query($filter)
  {
    if (null === $filter) {
      return [];
    }
    $dict = $filter;
    if ($dict instanceof Model) {
      $dict = $dict->toMap();
    }
    $tmp = [];
    foreach ($dict as $k => $v) {
      if (0 !== strpos($k, '_')) {
        $tmp[$k] = $v;
      }
    }

    return self::flatten($tmp);
  }

  /**
   * @remarks
   * Get signature according to signedParams, method and secret
   * 
   * @param string[] $signedParams Params which need to be signed
   * @param string $method HTTP method e.g. GET
   * @param string $secret AccessKeySecret
   * @return string The signature
   */
  static public function getRPCSignature($signedParams, $method, $secret)
  {
    $secret    .= '&';
    $strToSign = self::getRpcStrToSign($method, $signedParams);

    $signMethod = 'HMAC-SHA1';

    return self::encode($signMethod, $strToSign, $secret);
  }

  /**
   * @remarks
   * Parse array into a string with specified style
   * 
   * @param mixed $object The array
   * @param string $prefix The prefix string
   * @param string $style The style
   * @return string The string
   */
  static public function arrayToStringWithSpecifiedStyle($object, $prefix, $style)
  {
    if (null === $object) {
      return '';
    }
    if ('repeatList' === $style) {
      return self::toForm([$prefix => $object]);
    }
    if ('simple' == $style || 'spaceDelimited' == $style || 'pipeDelimited' == $style) {
      $strs = self::flatten($object);

      switch ($style) {
        case 'spaceDelimited':
          return implode(' ', $strs);

        case 'pipeDelimited':
          return implode('|', $strs);

        default:
          return implode(',', $strs);
      }
    } elseif ('json' === $style) {
      self::parse($object, $parsed);
      return json_encode($parsed);
    }

    return '';
  }

  /**
   * @remarks
   * Transform input as map.
   * @param mixed $input
   * @return mixed[]
   */
  static public function parseToMap($input)
  {
    self::parse($input, $result);
    return $result;
  }

  /**
   * @remarks
   * Get the authorization
   * 
   * @param Request $request Request params
   * @param string $signatureAlgorithm The autograph method
   * @param string $payload The hashed request
   * @param string $accesskey The accesskey string
   * @param string $accessKeySecret The accessKeySecret string
   * @return string Authorization string
   */
  static public function getAuthorization($request, $signatureAlgorithm, $payload, $accesskey, $accessKeySecret)
  {
    $canonicalURI = $request->pathname ? $request->pathname : '/';
    $query    = $request->query ?: [];
    $method               = strtoupper($request->method);
    $canonicalQueryString = self::getCanonicalQueryString($query);
    $signHeaders          = [];
    foreach ($request->headers as $k => $v) {
      $k = strtolower($k);
      if (0 === strpos($k, 'x-acs-') || 'host' === $k || 'content-type' === $k) {
        $signHeaders[$k] = $v;
      }
    }
    ksort($signHeaders);
    $headers = [];
    foreach ($request->headers as $k => $v) {
      $k = strtolower($k);
      if (0 === strpos($k, 'x-acs-') || 'host' === $k || 'content-type' === $k) {
        $headers[$k] = trim($v);
      }
    }
    $canonicalHeaderString = '';
    ksort($headers);
    foreach ($headers as $k => $v) {
      $canonicalHeaderString .= $k . ':' . trim(self::filter($v)) . "\n";
    }
    if (empty($canonicalHeaderString)) {
      $canonicalHeaderString = "\n";
    }

    $canonicalRequest = $method . "\n" . $canonicalURI . "\n" . $canonicalQueryString . "\n" .
        $canonicalHeaderString . "\n" . implode(';', array_keys($signHeaders)) . "\n" . $payload;
    $strtosign        = $signatureAlgorithm . "\n" . bin2hex(self::hash(BytesUtil::from($canonicalRequest), $signatureAlgorithm));

    $signature        = self::sign($accessKeySecret, $strtosign, $signatureAlgorithm);
    $signature        = bin2hex($signature);

    return $signatureAlgorithm .
      ' Credential=' . $accesskey .
      ',SignedHeaders=' . implode(';', array_keys($signHeaders)) .
      ',Signature=' . $signature;
  }

  /**
   * @param string $userAgent
   * @return string
   */
  static public function getUserAgent($userAgent)
  {
    if (empty(self::$defaultUserAgent)) {
      self::$defaultUserAgent = sprintf('AlibabaCloud (%s; %s) PHP/%s Core/1.0 TeaDSL/2', PHP_OS, \PHP_SAPI, PHP_VERSION);
    }
    if (!empty($userAgent)) {
      return self::$defaultUserAgent . ' ' . $userAgent;
    }

    return self::$defaultUserAgent;
  }

  private static function filter($str)
  {
    return str_replace(["\t", "\n", "\r", "\f"], '', $str);
  }

  public static function sign($secret, $str, $algorithm)
  {
    $result = '';
    switch ($algorithm) {
      case 'ACS3-HMAC-SHA256':
        $result = hash_hmac('sha256', $str, $secret, true);
        break;
      case 'ACS3-HMAC-SM3':
        $result = self::hmac_sm3($str, $secret, true);
        break;
      case 'ACS3-RSA-SHA256':
        $privateKey = "-----BEGIN RSA PRIVATE KEY-----\n" . $secret . "\n-----END RSA PRIVATE KEY-----";
        @openssl_sign($str, $result, $privateKey, OPENSSL_ALGO_SHA256);
    }

    return $result;
  }

  private static function hmac_sm3($data, $key, $raw_output = false)
  {
    $pack      = 'H' . \strlen(self::sm3('test'));
    $blocksize = 64;
    if (\strlen($key) > $blocksize) {
      $key = pack($pack, self::sm3($key));
    }
    $key  = str_pad($key, $blocksize, \chr(0x00));
    $ipad = $key ^ str_repeat(\chr(0x36), $blocksize);
    $opad = $key ^ str_repeat(\chr(0x5C), $blocksize);
    $hmac = self::sm3($opad . pack($pack, self::sm3($ipad . $data)));

    return $raw_output ? pack($pack, $hmac) : $hmac;
  }

  private static function sm3($message)
  {
      return (new Sm3())->sign($message);
  }

  private static function encode($signMethod, $strToSign, $secret)
  {
    switch ($signMethod) {
      case 'HMAC-SHA256':
        return base64_encode(hash_hmac('sha256', $strToSign, $secret, true));
      default:
        return base64_encode(hash_hmac('sha1', $strToSign, $secret, true));
    }
  }

  /**
     * @param array  $items
     * @param string $delimiter
     * @param string $prepend
     *
     * @return array
     */
  private static function flatten($items = [], $delimiter = '.', $prepend = '')
  {
    $flatten = [];

    foreach ($items as $key => $value) {
      $pos = \is_int($key) ? $key + 1 : $key;

      if ($value instanceof Model) {
        $value = $value->toMap();
      } elseif (\is_object($value)) {
        $value = get_object_vars($value);
      }

      if (\is_array($value) && !empty($value)) {
        $flatten = array_merge(
          $flatten,
          self::flatten($value, $delimiter, $prepend . $pos . $delimiter)
        );
      } else {
        if (\is_bool($value)) {
          $value = true === $value ? 'true' : 'false';
        }
        $flatten[$prepend . $pos] = $value;
      }
    }

    return $flatten;
  }

  private static function parse($input, &$output)
  {
    if (null === $input || '' === $input) {
      $output = [];
    }
    $recursive = function ($input) use (&$recursive) {
      if ($input instanceof Model) {
        $input = $input->toMap();
      } elseif (\is_object($input)) {
        $input = get_object_vars($input);
      }
      if (!\is_array($input)) {
        return $input;
      }
      $data = [];
      foreach ($input as $k => $v) {
        $data[$k] = $recursive($v);
      }

      return $data;
    };
    $output = $recursive($input);
    if (!\is_array($output)) {
      $output = [$output];
    }
  }

  private static function getRpcStrToSign($method, $query)
  {
    ksort($query);

    $params = [];
    foreach ($query as $k => $v) {
      if (null !== $v) {
        $k = rawurlencode($k);
        $v = rawurlencode($v);
        $params[] = $k . '=' . (string)$v;
      }
    }
    $str = implode('&', $params);

    return $method . '&' . rawurlencode('/') . '&' . rawurlencode($str);
  }

  private static function getCanonicalQueryString($query)
  {
    ksort($query);

    $params = [];
    foreach ($query as $k => $v) {
      if (null === $v) {
        continue;
      }
      $str = rawurlencode($k);
      if ('' !== $v && null !== $v) {
        $str .= '=' . rawurlencode($v);
      } else {
        $str .= '=';
      }
      $params[] = $str;
    }

    return implode('&', $params);
  }

  /**
   * Transform input as array.
   *
   * @param mixed $input
   *
   * @return array
  */
  public static function toArray($input)
  {
      if (\is_array($input)) {
          foreach ($input as $k => &$v) {
              $v = self::toArray($v);
          }
      } elseif ($input instanceof Model) {
          $input = $input->toMap();
          foreach ($input as $k => &$v) {
              $v = self::toArray($v);
          }
      }

      return $input;
  }

  /**
     * Stringify the value of map.
     *
     * @param array $map
     *
     * @return array the new stringified map
     */
    public static function stringifyMapValue($map)
    {
        if (null === $map) {
            return [];
        }
        foreach ($map as &$node) {
            if (is_numeric($node)) {
                $node = (string) $node;
            } elseif (null === $node) {
                $node = '';
            } elseif (\is_bool($node)) {
                $node = true === $node ? 'true' : 'false';
            } elseif (\is_object($node)) {
                $node = json_decode(json_encode($node), true);
            }
        }

        return $map;
    }
    /**
     * @param string   $product
     * @param string   $regionId
     * @param string   $endpointRule
     * @param string   $network
     * @param string   $suffix
     *
     * @return string
     */
    static public function getEndpointRules($product, $regionId, $endpointType, $network, $suffix = null) {
        $product = $product ?: "";
        $network = $network ?: "";
        
        if ($endpointType == "regional") {
            if ($regionId === null || $regionId === "") {
                throw new RuntimeException("RegionId is empty, please set a valid RegionId");
            }
            $result = "<product><network>.<region_id>.aliyuncs.com";
            $result = str_replace("<region_id>", $regionId, $result);
        } else {
            $result = "<product><network>.aliyuncs.com";
        }

        $result = str_replace("<product>", strtolower($product), $result);
        if ($network === "" || $network === "public") {
            $result = str_replace("<network>", "", $result);
        } else {
            $result = str_replace("<network>", "-" . $network, $result);
        }
        return $result;
    }
    
    /**
     * @param string   $product
     * @param string   $regionId
     * @param string   $endpointRule
     * @param string   $network
     * @param string   $suffix
     * @param string[] $endpointMap
     * @param string   $endpoint
     *
     * @return string
     */
     static function getProductEndpoint($product, $regionId, $endpointRule, $network, $suffix, $endpointMap, $endpoint)
    {
        if (null !== $endpoint) {
            return $endpoint;
        }

        if (null !== $endpointMap && null !== @$endpointMap[$regionId]) {
            return @$endpointMap[$regionId];
        }

        return self::getEndpointRules($product, $regionId, $endpointRule, $network, $suffix);
    }
}