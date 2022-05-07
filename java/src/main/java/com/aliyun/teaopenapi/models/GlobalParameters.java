// This file is auto-generated, don't edit it. Thanks.
package com.aliyun.teaopenapi.models;

import com.aliyun.tea.*;

public class GlobalParameters extends TeaModel {
    @NameInMap("headers")
    public java.util.Map<String, String> headers;

    @NameInMap("queries")
    public java.util.Map<String, String> queries;

    public static GlobalParameters build(java.util.Map<String, ?> map) throws Exception {
        GlobalParameters self = new GlobalParameters();
        return TeaModel.build(map, self);
    }

    public GlobalParameters setHeaders(java.util.Map<String, String> headers) {
        this.headers = headers;
        return this;
    }
    public java.util.Map<String, String> getHeaders() {
        return this.headers;
    }

    public GlobalParameters setQueries(java.util.Map<String, String> queries) {
        this.queries = queries;
        return this;
    }
    public java.util.Map<String, String> getQueries() {
        return this.queries;
    }

}
