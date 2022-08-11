
const main = require('./main');

//module.exports = function (callback, params) {

//    var createHash = function (params) {
//        var hasher = main.Poseidon.createHash(14, 6, 53)(params).toString(10);
//        return hasher;
//    }

//    callback(null, createHash(params));
//}


//module.exports = function (callback, key, msg) {

//    var sign = function (key, msg) {
//        var sign1 = JSON.stringify(main.EdDSA.sign(key, msg));
//        return sign1;
//    }

//    callback(null, sign(params));
//}

//function createHash111(params) {
//    var hasher = main.Poseidon.createHash(14, 6, 53)(params.split(",")).toString(10);
//    return hasher;
//}


function createHash(callback, params) {
    //console.log(params);
    var hasher = main.Poseidon.createHash(14, 6, 53)(params).toString(10);
    //callback(null, JSON.stringify(params));
    callback(null, hasher);
}


function sign(callback, key, msg) {
    var sign1 = JSON.stringify(main.EdDSA.sign(key, msg));
    callback(null, sign1);
}

module.exports = {
    createHash,
    sign
    //createHash111
}