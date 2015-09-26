$(document).ready(function() {
    var urlInput = $("#urlInput");
    var urlSubmitForm = $("#urlSubmitForm");

    function validateUrl(url) {
        return /^(http):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i.test(url);
    }

    function poll(jobId) {
        $.ajax({
            url: "/api/jobs/" + jobId,
            type: "GET",
            dataType: "json",
            contentType: "application/json"
        }).done(function(data) {
            var jobContainer = "#job" + jobId;
            $(jobContainer + " #jobStatus").text(data.status);

            if (data.result.length > 0) {
                $(jobContainer).append("<table id='jobResult' class='table'>" +
                    "<caption>Downloaded images</caption>" +
                    "<tr><th>Local URL</th>" +
                    "<th>Remote URL</th>" +
                    "<th>Width</th>" +
                    "<th>Height</th>" +
                    "<th>Size</th>" +
                    "<th>Content-type</th>" +
                    "</tr></table>");

                for (var i = 0; i < data.result.length; ++i) {
                    $(jobContainer + " #jobResult").append("<tr><td>" + "<a href=" + data.result[i].localUrl + " target='_blank'>...</a>" + "</td><td>" + "<a href=" + data.result[i].remoteUrl + " target='_blank'>...</a>" + "</td><td>" + data.result[i].width + "</td><td>" + data.result[i].height + "</td><td>" + data.result[i].size + "</td><td>" + data.result[i].contentType + "</td></tr>");
                }
            }

            if (data.status !== "Succeeded" && data.status !== "Failed" && data.status !== "Deleted") {
                setTimeout(function() {
                    poll(jobId);
                }, 2000);
            }
        }).fail(function() {
            alert("Ooops, something went wrong");
        });
    };

    $("form").submit(function(event) {
        event.preventDefault();

        var url = urlInput.val();
        var isValidUrl = validateUrl(url);

        if (isValidUrl === true) {
            urlSubmitForm.removeClass("has-error");

            $.ajax({
                type: "POST",
                url: "/api/jobs",
                data: JSON.stringify({ value: url }),
                dataType: "json",
                contentType: "application/json"
            }).done(function(jobId) {
                alert("Job was created. Job Id: " + jobId);

                $("#jobList").append("<div id=" + "job" + jobId + "><div><span id='jobId'><strong>Job Id: </strong>" + jobId + " (" + "<a href=" + url + " target='_blank'>" + url + "</a>" + ")</span></div>" +
                    "<div><strong>Job status: </strong><span id='jobStatus'></span></div></div><hr/>");

                poll(jobId);
            }).fail(function() {
                alert("Ooops, something went wrong");
            });
        } else {
            urlSubmitForm.addClass("has-error");
        }
    });
});