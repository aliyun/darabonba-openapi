// This file is auto-generated, don't edit it. Thanks.
package com.aliyun.teaopenapi.models;

import com.aliyun.tea.*;

public class OpenApiRequest extends TeaModel {
    @NameInMap("headers")
    public java.util.Map<String, String> headers;

    @NameInMap("query")
    public java.util.Map<String, String> query;

    @NameInMap("body")
    public Object body;

    public static OpenApiRequest build(java.util.Map<String, ?> map) throws Exception {
        OpenApiRequest self = new OpenApiRequest();
        return TeaModel.build(map, self);
    }

    public OpenApiRequest setHeaders(java.util.Map<String, String> headers) {
        this.headers = headers;
        return this;
    }
    public java.util.Map<String, String> getHeaders() {
        return this.headers;
    }

    public OpenApiRequest setQuery(java.util.Map<String, String> query) {
        this.query = query;
        return this;
    }
    public java.util.Map<String, String> getQuery() {
        return this.query;
    }

    public OpenApiRequest setBody(Object body) {
        this.body = body;
        return this;
    }
    public Object getBody() {
        return this.body;
    }

}
