function auctionItem(id, name, desc, price, bidder, start, end, dur) {
    this.Id = id;
    this.Name = name;
    this.Desc = desc;
    this.Price = price;
    this.Bidder = bidder;
    this.Start = start;
    this.End = end;
    this.Bid = "";
}

$(document).ready(function () {

    var con = $.hubConnection();
    var hub = con.createHubProxy('myHub');

    hub.on('Message', function (msg) {
        $("#messages").html($("#messages").html() + "<br />" + msg);
    });

    hub.on('Reload', function () {
        vm.loadItems();
    });

    con.start().done(function () {
        hub.invoke('Send', vm.username(), "Has joined the Auction");
        });

    var vm = {

        username: ko.observable(getQSVar("u")),
        items: ko.observableArray([]),
        message: ko.observable(""),
        hitCount: ko.observable(0),

        loadItems: function () {
            vm.items.removeAll();
            $.getJSON("/api/items/started", function (data) {
                var array = vm.items();
                // ko.utils.arrayPushAll(array, data);
                $.each(data, function (indexInArray, item) {
                    array.push(new auctionItem(item.id, item.name, item.desc, item.price, item.bidder, item.start, item.end, item.duration));
                });
                vm.items.valueHasMutated();
            }, function (err) {
                dump(err);
            });
        },

        postMessage: function () {
            hub.invoke('Send', vm.username(), vm.message());
            vm.message("");
        },

        bid: function (item) {
            if (item.Bid <= item.Price) {
                alert("Bid must be greater than the current price");
            }
            else {
                hub.invoke("Bid", vm.username(), item.Id, accounting.formatMoney(item.Bid));
                //$.getJSON("/api/update/" + item.Id + "/" + item.Bid, function (data) {
                //    hub.invoke('Bid', item.Name, item.Id, accounting.formatMoney(item.Bid));
                //});
            }
        }
    };

    // Activates knockout.js
    ko.applyBindings(vm);

    vm.loadItems();
});

function getQSVar(sVar) {
    return unescape(window.location.search.replace(new RegExp("^(?:.*[&\\?]" + escape(sVar).replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"));
}
