$(document).ready(function () {

    var vm = {

        username: ko.observable(""),

        joinAuction: function () {
            document.location.href = "/Home/Auction?u=" + vm.username();
        }
    };

    // Activates knockout.js
    ko.applyBindings(vm);
});