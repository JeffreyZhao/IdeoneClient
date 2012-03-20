var Task = Jscex.Async.Task;

$.ajaxAsync = function (options) {
    return Task.create(function (t) {

        options.success = function (data, textStatus, jqXHR) {
            t.complete("success", data);
        }

        options.error = function (xhr, textStatus, errorThrow) {
            var ex;

            if (xhr.getResponseHeader("Content-Type") == "application/json") {
                var ex = $.parseJSON(xhr.responseText);
            } else {
                var ex = { message: errorThrow };
            }

            ex.source = "xhr";
            t.complete("failure", ex);
        };

        $.ajax(options);
    });
};