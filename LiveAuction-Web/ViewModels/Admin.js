$(document).ready(function () {

    var vm = {

        items: ko.observableArray([]),

        loadItems: function () {
            vm.items.removeAll();
            $.getJSON("/api/items?status=All", function (data) {
                var array = vm.items();
                ko.utils.arrayPushAll(array, data);
                vm.items.valueHasMutated();
            }, function (err) {
                dump(err);
            });
        }

    };

    // Activates knockout.js
    ko.applyBindings(vm);

    vm.loadItems();
});