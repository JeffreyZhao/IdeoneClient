var Task = Jscex.Async.Task;

$.ajaxAsync = function (options) {
    return Task.create(function (t) {

        options.success = function (data, textStatus, jqXHR) {
            t.complete("success", data);
        }

        options.error = function (xhr, textStatus, errorThrow) {
            if (xhr.getResponseHeader("Content-Type") == "application/json") {
                t.complete("failure", $.parseJSON(xhr.responseText));
            } else {
                t.complete("failure", xhr.responseText);
            }
        };

        $.ajax(options);
    });
};