#ifndef ALIBABACLOUD_GATEWAY_ATTRIBUTEMAP_H_
#define ALIBABACLOUD_GATEWAY_ATTRIBUTEMAP_H_

#include <darabonba/Model.hpp>

namespace AlibabaCloud {
  namespace Gateway {
    namespace Models {
      class AttributeMap : public Darabonba::Model {
        friend void to_json(Darabonba::Json &j, const AttributeMap &obj) {
          DARABONBA_ANY_TO_JSON(attributes, attributes_);
          DARABONBA_PTR_TO_JSON(key, key_);
        }

        friend void from_json(const Darabonba::Json &j, AttributeMap &obj) {
          DARABONBA_ANY_FROM_JSON(attributes, attributes_);
          DARABONBA_PTR_FROM_JSON(key, key_);
        }

      public:
        AttributeMap() = default;

        AttributeMap(const AttributeMap &) = default;

        AttributeMap(AttributeMap &&) = default;

        AttributeMap &operator=(const AttributeMap &) = default;

        AttributeMap &operator=(AttributeMap &&) = default;

        AttributeMap(const Darabonba::Json &obj) { from_json(obj, *this); }

        virtual ~AttributeMap() = default;

        virtual void validate() const override {}

        virtual void fromMap(const Darabonba::Json &obj) override {
          from_json(obj, *this);
          validate();
        }

        virtual Darabonba::Json toMap() const override {
          Darabonba::Json obj;
          to_json(obj, *this);
          return obj;
        }

        virtual bool empty() const override {
          return attributes_ == nullptr && key_ == nullptr;
        }

        bool hasAttributes() const { return this->attributes_ != nullptr; }

        const Darabonba::Json &attributes() const { DARABONBA_GET(attributes_); }

        Darabonba::Json &attributes() { DARABONBA_GET(attributes_); }

        AttributeMap &setAttributes(const Darabonba::Json &attributes) {
          DARABONBA_SET_VALUE(attributes_, attributes);
        }

        AttributeMap &setAttributes(Darabonba::Json &&attributes) {
          DARABONBA_SET_RVALUE(attributes_, attributes);
        }

        bool hasKey() const { return this->key_ != nullptr; }

        const std::map<std::string, std::string> &key() const {
          DARABONBA_PTR_GET_CONST(key_, std::map<std::string, std::string>);
        }

        std::map<std::string, std::string> getKey() { DARABONBA_PTR_GET(key_, std::map<std::string, std::string>); }

        AttributeMap &setKey(const std::map<std::string, std::string> &key) {
          DARABONBA_PTR_SET_VALUE(key_, key);
        }

        AttributeMap &setKey(std::map<std::string, std::string> &&key) {
          DARABONBA_PTR_SET_RVALUE(key_, key);
        }

      protected:
        Darabonba::Json attributes_ = nullptr;
        std::shared_ptr<std::map<std::string, std::string>> key_ = nullptr;
      };
    }
  } // namespace Gateway
} // namespace AlibabaCloud

#endif