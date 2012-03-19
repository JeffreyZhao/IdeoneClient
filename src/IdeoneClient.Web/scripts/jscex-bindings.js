var Task = Jscex.Async.Task;

$.ajaxAsync = function (options) {
    return Task.create(function (t) {

        options.success = function (data, textStatus, jqXHR) {
            t.complete("success", data);
        }

        options.error = function (jqXHR, textStatus, errorThrow) {
            t.complete("failure", {
                jqXHR: jqXHR,
                textStatus: textStatus,
                errorThrow: errorThrow
            });
        };

        $.ajax(options);
    });
};