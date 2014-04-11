var SquareHook = SquareHook || {};

SquareHook.Certification = (function ($) {
    var my = {};

    my.providers = null;
    my.careers = null;
    my.selectedCareer = null;
    my.careerDetails = null;
    my.selectedProviders = null;

    my.BaseUrl = "/API/";

    my.Initialize = function () {
        my.Start();

        $(".sh-career-list").on("click", "a", my.SelectCareer);
        $(".sh-details .levels").on("click", ".level-details > h3", my.ToggleCertifications)
        $(".sh-details .levels").on("hover", "li", my.CertificationDetails);
        $(".sh-provider-list").on("click", ".select-all", my.SelectAllProviders);
        $(".sh-provider-list").on("click", ".select-none", my.SelectNoProviders);

        $(".sh-show-more").click(function (e) {
            var providers = $(".sh-provider-list li.provider-toggle").toggle();

            if ($(this).html() == "SEE MORE...") {
                $(this).html("SEE LESS...");
            }
            else {
                $(this).html("SEE MORE...");
            }
            e.preventDefault();
            return false;
        });

        $(".sh-view-all").click(my.ViewAll);
    };

    my.SelectAllProviders = function (e) {
        $(".check").prop("checked", true);
        my.refreshProviders();
        e.preventDefault();
        return false;
    };

    my.SelectNoProviders = function (e) {
        $(".check").prop("checked", false);
        my.refreshProviders();
        e.preventDefault();
        return false;
    };

    my.getSelectedProviders = function () {
        my.selectedProviders = $(".sh-provider-list input:checkbox:checked").map(function () {
            return eval($(this).val());
        }).get();

        return my.selectedProviders;
    };

    my.providerSelected = function (id) {
        var found = false;
        for (var i = 0; i < my.selectedProviders.length; i++) {
            if (id == my.selectedProviders[i]) {
                found = true;
                break;
            }
        }

        return found;
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

                $(".sh-provider-list input[type='checkbox']").click(my.refreshProviders);
                my.getSelectedProviders();
            }
            else { alert("Error Initializing"); }
        });
    };

    my.refreshProviders = function (e) {
        var providers = my.getSelectedProviders();
        var data = { success: true, results: my.careerDetails, providers: my.providers };
        my.ProcessDetails(data);
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

        $.post(my.BaseUrl + "CareerDetails", { id: id }, my.processCareerDetails);

        // if small screen scroll
        if ($(window).width() < 980) {
            $('html, body').animate({
                scrollTop: $("#shGoto").offset().top
            }, 500);
        }

        // hide start
        $(".sh-start").hide();
        $(".sh-details section.header").show();
        $(".sh-details section.levels").show();

        e.preventDefault();
        return false;
    };

    my.processCareerDetails = function (results) {
        my.providers = results.providers;
        var plist = Mustache.render($("#shProviderListTemplate").html(), my);
        $(".sh-provider-list").html(plist);
        $(".sh-provider-list input[type='checkbox']").click(my.refreshProviders);
        my.selectedProviders = my.getSelectedProviders();

        if (my.providers.length > 10) {
            $(".sh-show-more").show();
        } else {
            $(".sh-show-more").hide();
        }
        my.ProcessDetails(results);
    };

    my.ViewAll = function (e) {
        $(".sh-career-list li").removeClass("active");

        if (my.selectedCareer != null) {
            $(".sh-details .header > h2").text("All Careers");
            $(".sh-details .header .average-salary").text("N/A");
            $(".sh-details .header .demand").text("N/A");
        }

        $.post(my.BaseUrl + "ViewAll", null, my.processCareerDetails);
        e.preventDefault();
        return false;
    };

    my.ProcessDetails = function (results) {
        if (results.success) {
            my.careerDetails = results.results;

            // set visible provider certs
            if (my.careerDetails != null) {
                for (var l = 0; l < my.careerDetails.Levels.length; l++) {
                    var level = my.careerDetails.Levels[l];
                    for (var c = 0; c < level.Certifications.length; c++) {
                        level.Certifications[c].Visible = my.providerSelected(level.Certifications[c].ProviderID);
                    }
                }

                $(".popover").remove();

                if (my.careerDetails.Levels != null && my.careerDetails.Levels.length > 0) {
                    my.careerDetails.Levels[0].First = true;
                }
                var output = Mustache.render($("#shDetails").html(), my.careerDetails);
                $(".sh-details .levels").html(output);
            }

            $(".instruction").popover({
                html: true,
                placement: 'right'
            })

            $(".instruction.active").popover('show');

            /*$(".instruction").on('shown.bs.popover', function (element) {
            var popover = $("#instructionContainer .popover")
            var height = popover.height() / 3;
            var top = popover.position().top;
            popover.css("top", (top + height) + "px");

            console.log("yay");
            });*/

            $(".sh-details .levels li > a").popover({
                html: true,
                placement: 'right',
                trigger: 'hover',
                container: 'body'
            });
        }
    };

    my.ToggleCertifications = function (e) {
        $(this).parent().find("ul").slideToggle();

        var icon = $(this).find(".shicon");
        if (icon.hasClass("shicon-plus")) {
            icon.removeClass("shicon-plus").addClass("shicon-minus");
        }
        else {
            icon.removeClass("shicon-minus").addClass("shicon-plus");
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