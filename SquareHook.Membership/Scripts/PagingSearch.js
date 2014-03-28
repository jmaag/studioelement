var Leuly = Leuly || {};

Leuly.PagingSearch = (function ($) {
    var my = {};

    my.template = "";
    my.pagingTemplate = "<ul class='pagination'><li class=\"prev\"><a href=\"Prev\">Prev</a></li>" +
        "{{#pages}}<li><a class=\"page_{{.}}\" href=\"{{.}}\">{{.}}</a></li>{{/pages}}" +
        "<li class=\"next\"><a href=\"Next\">Next</a></li></ul>";
    my.page = 1;
    my.take = 10;
    my.total = 0;
    my.search = "";
    my.searchUrl = "";

    my.Initialize = function (url, templateId) {
        my.searchUrl = url;
        my.template = $(templateId).html();
        my.Search();

        $("form.table-search").submit(function () {
            my.page = 1;
            my.search = $("form.table-search input[type=text]").val();
            my.Search();
            return false;
        });

        $(".data-table").on("click", ".delete", my.DeleteCall);
    };

    my.Search = function () {
        var data = { take: my.take, page: my.page, search: my.search };
        $.post(my.searchUrl, data, function (results) {
            results.LastLoginInWords = function () {
                var date = my.parseJsonDate(this.LastLoginDate);
                return $.timeago(date);
            };

            var output = "";
            if (results.results.length > 0) {
                output = Mustache.render(my.template, results);
            }

            // adjust the paging
            my.total = results.total;
            my.RenderPaging();

            $(".data-table tbody").html(output);
        }, "json");
    };

    my.Insert = function (item) {
        var data = { results: [item] };
        var output = Mustache.render(my.template, data);
        $(".data-table tbody").append(output);

    };

    my.DeleteCall = function (e) {
        e.preventDefault();
        if (confirm("Are you sure you want to delete?")) {
            var url = $(this).attr("href");
            var id = $(this).parent().parent().attr("id");

            $.post(url, function (results) {
                if (results.success) {
                    $("#" + id).remove();
                }
            });
        }
        return false;
    };

    my.parseJsonDate = function (jsonDateString) {
        return new Date(parseInt(jsonDateString.replace('/Date(', '')));
    };

    my.RenderPaging = function () {
        var startPage = my.page;
        while (startPage % 5 > 1) {
            startPage--;
        }

        var pages = [];
        for (var i = 0; i < 5; i++) {
            if ((startPage + i - 1) * my.take < my.total) {
                pages.push(startPage + i);
            }
        }

        var output = $(Mustache.render(my.pagingTemplate, { "pages": pages }));
        output.find(".page_" + my.page).parent().addClass("active");
        output.find("li").removeClass("disabled");
        // check to disable previous
        if (my.page == 1) {
            output.find("li.prev").addClass("disabled");
        }

        // check to disable next
        if (my.page * my.take > my.total) {
            output.find("li.next").addClass("disabled");
        }

        // first check to see if the number is already in the list
        if (output.find("a.page_" + my.page).length > 0) {
            output.find("li").removeClass("active");
            output.find("a.page_" + my.page).parent().addClass("active");
        }

        // bind event
        output.find("a").click(function () {
            if (!($(this).parent().hasClass("disabled"))) {
                var same = false;
                var url = $(this).attr("href");
                if (url == "Prev") { my.page--; }
                else if (url == "Next") { my.page++; }
                else {
                    same = (my.page == eval(url));
                    my.page = eval(url);
                }

                if (!same) { my.Search(); }
            }
            return false;
        });

        $(".pagination").html(output);
    };

    return my;

} (jQuery));