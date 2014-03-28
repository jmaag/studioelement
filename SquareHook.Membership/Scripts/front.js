var SquareHook = SquareHook || {};

SquareHook.Certification = (function ($) {
    var my = {};

    my.providers = null;
    my.careers = null;
    my.selectedCareer = null;
    my.careerDetails = null;

    my.BaseUrl = "/API/";

    my.Initialize = function () {
        my.Start();

        $(".sh-career-list").on("click", "a", my.SelectCareer);
        $(".sh-details .levels").on("click", ".level-details > h3", my.ToggleCertifications)
        $(".sh-details .levels").on("hover", "li", my.CertificationDetails);

        $(".sh-show-more").click(function (e) {
            $(".sh-provider-list li").removeAttr("style");
            $(this).hide();
            e.preventDefault();
            return false;
        });

        $(".sh-view-all").click(my.ViewAll);
    };

    my.Start = function () {
        $.post(my.BaseUrl + "Start", null, function (results) {
            if (results.success) {
                my.providers = results.providers;
                my.careers = results.careers;

                var clist = Mustache.render($("#shCareerItemTemplate").html(), my);
                $(".sh-career-list").html(clist);

                var plist = Mustache.render($("#shProviderListTemplate").html(), my);
                $(".sh-provider-list").html(plist);
            }
            else { alert("Error Initializing"); }
        });
    };

    my.SelectCareer = function (e) {
        var element = $(this);
        var id = element.attr("data-id");

        $(".sh-career-list li").removeClass("active");
        element.closest("li").addClass("active");

        // find the career
        for (var i = 0; i < my.careers.length; i++) {
            if (id == my.careers[i].CareerID) {
                my.selectedCareer = my.careers[i];
                break;
            }
        }

        if (my.selectedCareer != null) {
            $(".sh-details .header > h2").text(my.selectedCareer.Name);
            $(".sh-details .header .average-salary").text(my.selectedCareer.AverageSalary);
            $(".sh-details .header .demand").text(my.selectedCareer.Demand);
        }

        $.post(my.BaseUrl + "CareerDetails", { id: id }, my.ProcessDetails);

        e.preventDefault();
        return false;
    };

    my.ViewAll = function (e) {
        $(".sh-career-list li").removeClass("active");

        if (my.selectedCareer != null) {
            $(".sh-details .header > h2").text("All Careers");
            $(".sh-details .header .average-salary").text("N/A");
            $(".sh-details .header .demand").text("N/A");
        }

        $.post(my.BaseUrl + "ViewAll", null, my.ProcessDetails);
        e.preventDefault();
        return false;
    };

    my.ProcessDetails = function (results) {
        if (results.success) {
            my.careerDetails = results.results;
            var output = Mustache.render($("#shDetails").html(), my.careerDetails);
            $(".sh-details .levels").html(output);

            $(".sh-details .levels li > a").popover({
                html: true,
                placement: 'right',
                trigger: 'hover',
                container: 'body'
            });
        }
    };

    my.ToggleCertifications = function (e) {
        $(this).next().toggle();

        var icon = $(this).find(".fa");
        if (icon.hasClass("fa-plus")) {
            icon.removeClass("fa-plus").addClass("fa-minus");
        }
        else {
            icon.removeClass("fa-minus").addClass("fa-plus");
        }
    };

    my.CertificationDetails = function () {
        var item = $(this);
    };

    my.parseJsonDate = function (jsonDateString) {
        return new Date(parseInt(jsonDateString.replace('/Date(', '')));
    };

    return my;

} (jQuery));

$(function () {
    SquareHook.Certification.Initialize();
});