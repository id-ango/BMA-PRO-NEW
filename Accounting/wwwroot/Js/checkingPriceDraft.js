window.checkingPriceDraftGet = function (key) {
    return sessionStorage.getItem(key);
};

window.checkingPriceDraftSet = function (key, value) {
    sessionStorage.setItem(key, value);
};
