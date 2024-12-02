mergeInto(LibraryManager.library, {
    //Get JTW token
    RequestToken: function (apiUrl) {
        var link = UTF8ToString(apiUrl);
        window.getToken(link);
    },
    RequestBoosterPayment: function (apiUrl, token) {
        var link = UTF8ToString(apiUrl);
        var token = UTF8ToString(token);
        window.fetchInvoiceLink(link, token);
    },
    //Invite friends
    Invite: function (link)
    {
        var inviteLink = UTF8ToString(link);
        if (navigator.share) {
            navigator.share({
                title: 'Title of the content to share',
                text: 'Text of the content to share',
                url: inviteLink, // URL to share
            })
                .then(() => console.log('Successful share'))
                .catch((error) => console.log('Error sharing', error));
        } else {
            console.log('Web Share API is not supported in your browser.');
        }
    },
    //Copy link to clipboard
    CopyLink: function (link)
    {
        var text = UTF8ToString(link); // Замените на текст, который вы хотите скопировать
        var tempInput = document.createElement("input");
        tempInput.style = "position: absolute; left: -1000px; top: -1000px";
        tempInput.value = text;
        document.body.appendChild(tempInput);
        tempInput.select();
        document.execCommand("copy");
        document.body.removeChild(tempInput);
        console.log('Copying to clipboard was successful!');
    },
    AdsgramShow: function (type)
    {
        var blockId = UTF8ToString(type);
        window.adsShow(blockId);
    },
    TonTonEvent: function (screenNameUtf, userTgIdUtf, userRefUtf, userUtmUtf) {

        try {
            var screenName = UTF8ToString(screenNameUtf);
            var userTgId = UTF8ToString(userTgIdUtf);
            var userRef = UTF8ToString(userRefUtf);
            var userUtm = UTF8ToString(userUtmUtf);
            window.gaEvent(screenName, userTgId, userRef, userUtm);
        } catch (err) {
            console.log(err)
        }

    },
    OpenURL: function (link)
    {
        var url = UTF8ToString(link);
        document.onmouseup = function()
        {
            window.open(url);
            document.onmouseup = null;
        }

        document.ontouchend = function()
        {
            window.open(url);
            document.ontouchend = null;
        }
    }
});