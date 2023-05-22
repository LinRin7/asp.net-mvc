// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//_Layout.cshtml

function showMessage() {//alert從action帶到頁面的ViewBag.Message
    var message = $("#Message").val();
    if (message != null && message != "") {
        alert(message);
    }
    console.log(message);
}
//ViewBag帶到前端的值放入input
//<div>
//    <input type="hidden" value="@ViewBag.Message" id="Message" />
//</div>

//js
//window.onload = function () {
//    showMessage();
//};

function login() {
    const url = '/Members/Login';
    window.location.href = url;
}

function register() {
    const url = '/Members/Register';
    window.location.href = url;
}

function memberCenter() {
    const url = '/Members/index';
    window.location.href = url;
}

function logout() {
    const url = '/Members/Logout';
    window.location.href = url;
}

