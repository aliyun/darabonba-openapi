// This file is auto-generated, don't edit it. Thanks.
package com.aliyun.teaopenapi.models;

import com.aliyun.tea.*;

public class OpenApiRequest extends TeaModel {
    @NameInMap("headers")
    public java.util.Map<String, String> headers;

    @NameInMap("query")
    public java.util.Map<String, String> query;

    @NameInMap("body")
    public java.util.Map<String, ?> body;

    public static OpenApiRequest build(java.util.Map<String, ?> map) throws Exception {
        OpenApiRequest self = new OpenApiRequest();
        return TeaModel.build(map, self);
    }

}
