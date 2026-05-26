#ifndef ALIBABACLOUD_CODE_H_
#define ALIBABACLOUD_CODE_H_

#include <darabonba/Model.hpp>
#include <darabonba/Type.hpp>
#include <darabonba/http/MCurlResponse.hpp>
#include <darabonba/http/Request.hpp>
#include <string>

namespace Alibabacloud {

class StreamJson {

  friend void to_json(Darabonba::Json &j, const StreamJson &obj) {
    if (obj.type_ == STREAM || obj.type_ == UNSET) {
      j = nullptr;
    } else if (obj.type_ == JSON) {
      j = obj.json_;
    }
  }
  friend void from_json(const Darabonba::Json &j, StreamJson &obj) {
    obj.json_ = j;
    obj.type_ = JSON;
  }

public:
  StreamJson() = default;
  StreamJson(const StreamJson &) = default;
  StreamJson(StreamJson &&obj)
      : type_(obj.type_), stream_(std::move(obj.stream_)),
        json_(std::move(obj.json_)) {
    obj.type_ = UNSET;
  }
  StreamJson(std::shared_ptr<Darabonba::Http::MCurlResponseBody> stream)
      : type_(STREAM), stream_(stream) {}
  StreamJson(const Darabonba::Json &json) : type_(JSON), json_(json) {}
  StreamJson(Darabonba::Json &&json) : type_(JSON), json_(std::move(json)) {}

  StreamJson &operator=(const StreamJson &obj) {
    type_ = obj.type_;
    stream_ = obj.stream_;
    json_ = obj.json_;
    return *this;
  }
  StreamJson &operator=(StreamJson &&obj) {
    type_ = obj.type_;
    stream_ = std::move(obj.stream_);
    json_ = std::move(obj.json_);
    obj.type_ = UNSET;
    return *this;
  }
  StreamJson &
  operator=(std::shared_ptr<Darabonba::Http::MCurlResponseBody> stream) {
    json_ = nullptr;
    stream_ = stream;
    type_ = STREAM;
    return *this;
  }
  StreamJson &operator=(const Darabonba::Json &json) {
    stream_ = nullptr;
    json_ = json;
    type_ = JSON;
    return *this;
  }
  bool isStream() const { return type_ == STREAM; };
  bool isJson() const { return type_ == JSON; };

  std::shared_ptr<Darabonba::Http::MCurlResponseBody> stream() const {
    return stream_;
  }
  const Darabonba::Json &json() const { return json_; };
  Darabonba::Json &json() { return json_; }

  explicit operator Darabonba::Json() const { return json_; }
  explicit
  operator std::shared_ptr<Darabonba::Http::MCurlResponseBody>() const {
    return stream_;
  }

  bool empty() const {
    return type_ == UNSET || (type_ == JSON && json_.is_null()) ||
           (type_ == STREAM && stream_ == nullptr);
  }

protected:
  enum BodyType { STREAM, JSON, UNSET };
  BodyType type_ = UNSET;
  std::shared_ptr<Darabonba::Http::MCurlResponseBody> stream_ = nullptr;
  Darabonba::Json json_ = nullptr;
};


} // namespace Alibabacloud
#endif