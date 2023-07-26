// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('*[class^="activeVersion"]').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Measurement/AutoCompleteData',
                data: { "Prefix": request.term },
                dataType: "json",
                type: "POST",
                success: function (data) {
                    //debugger;
                    response($.map(data, function (item) {
                        return { label: item.name, value: item.name };
                    }))
                },
                messages: {
                    noResults: "", results: ""
                },
                error: function (xhr, textStatus, error) {
                    alert(xhr.statusText);
                },
                failure: function (response) {
                    alert("failure " + response.responseText);
                }
            });
        },
        minLength: 1
    });
});
