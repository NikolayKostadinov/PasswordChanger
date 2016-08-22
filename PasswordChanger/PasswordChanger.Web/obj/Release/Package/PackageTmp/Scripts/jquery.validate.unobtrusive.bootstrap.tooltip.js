(function ($) {
    var classes = { groupIdentifier: ".form-controll-wrapper", error: 'text-box single-line input-validation-error', success: 'text-box single-line' };//success: 'has-success'
    function updateClasses(inputElement, toAdd, toRemove) {
        var group = inputElement;
        if (group.length > 0) {
            group.removeClass(toRemove).addClass(toAdd);
        }
    }
    function onError(inputElement, message) {
        updateClasses(inputElement, classes.error, classes.success);

        var options = { placement: 'right', html: true, title: '<div class="tooltip-alert alert-danger">' + message + '</div>' };
        inputElement.tooltip("destroy")
			.addClass("input-validation-error")
			.tooltip(options);
    }
    function onSuccess(inputElement) {
        updateClasses(inputElement, classes.success, classes.error);
        inputElement.tooltip("destroy");
    }

    function onValidated(errorMap, errorList) {
        $.each(errorList, function () {
            onError($(this.element), this.message);
        });

        if (this.settings.success) {
            $.each(this.successList, function () {
                onSuccess($(this));
            });
        }
    }

    $(function () {
        $('form').each(function () {
            var validator = $(this).data('validator');
            validator.settings.showErrors = onValidated;
        });
    });
    $(function () {
        var errorSpans = $('span.field-validation-error');
        $.each(errorSpans, function (index, span) {
            var errorObj = $(span);
            var elementId = errorObj.data('valmsgFor');
            if (elementId !== '') {
                onError($('#' + elementId), errorObj.text());
                errorObj.remove();
            }
        });
    })
}(jQuery));


