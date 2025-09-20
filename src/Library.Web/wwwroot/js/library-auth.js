// Library Web Application JavaScript Functions

window.libraryAuth = {
    redirectTo: function (url) {
        window.location.href = url;
    },
    
    setLoginData: function (email, password) {
        localStorage.setItem('tempLoginEmail', email);
        localStorage.setItem('tempLoginPassword', password);
    },
    
    getLoginData: function () {
        const email = localStorage.getItem('tempLoginEmail');
        const password = localStorage.getItem('tempLoginPassword');
        return { email: email, password: password };
    },
    
    clearLoginData: function () {
        localStorage.removeItem('tempLoginEmail');
        localStorage.removeItem('tempLoginPassword');
    }
};
