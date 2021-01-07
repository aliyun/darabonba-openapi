// This file is auto-generated, don't edit it. Thanks.
package com.aliyun.teaopenapi.models;

import com.aliyun.tea.*;

public class Params extends TeaModel {
    @NameInMap("action")
    @Validation(required = true)
    public String action;

    @NameInMap("version")
    @Validation(required = true)
    public String version;

    @NameInMap("protocol")
    @Validation(required = true)
    public String protocol;

    @NameInMap("pathname")
    @Validation(required = true)
    public String pathname;

    @NameInMap("method")
    @Validation(required = true)
    public String method;

    @NameInMap("authType")
    @Validation(required = true)
    public String authType;

    @NameInMap("bodyType")
    @Validation(required = true)
    public String bodyType;

    @NameInMap("reqBodyType")
    @Validation(required = true)
    public String reqBodyType;

    @NameInMap("style")
    public String style;

    public static Params build(java.util.Map<String, ?> map) throws Exception {
        Params self = new Params();
        return TeaModel.build(map, self);
    }

    public Params setAction(String action) {
        this.action = action;
        return this;
    }
    public String getAction() {
        return this.action;
    }

    public Params setVersion(String version) {
        this.version = version;
        return this;
    }
    public String getVersion() {
        return this.version;
    }

    public Params setProtocol(String protocol) {
        this.protocol = protocol;
        return this;
    }
    public String getProtocol() {
        return this.protocol;
    }

    public Params setPathname(String pathname) {
        this.pathname = pathname;
        return this;
    }
    public String getPathname() {
        return this.pathname;
    }

    public Params setMethod(String method) {
        this.method = method;
        return this;
    }
    public String getMethod() {
        return this.method;
    }

    public Params setAuthType(String authType) {
        this.authType = authType;
        return this;
    }
    public String getAuthType() {
        return this.authType;
    }

    public Params setBodyType(String bodyType) {
        this.bodyType = bodyType;
        return this;
    }
    public String getBodyType() {
        return this.bodyType;
    }

    public Params setReqBodyType(String reqBodyType) {
        this.reqBodyType = reqBodyType;
        return this;
    }
    public String getReqBodyType() {
        return this.reqBodyType;
    }

    public Params setStyle(String style) {
        this.style = style;
        return this;
    }
    public String getStyle() {
        return this.style;
    }

}
