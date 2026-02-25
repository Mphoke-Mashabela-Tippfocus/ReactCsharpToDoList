window.onload = function () {
    const originalAuthorize = window.ui.authActions.authorize;
    window.ui.authActions.authorize = function (security) {
        if (security && security.Bearer && security.Bearer.value && !security.Bearer.value.startsWith("Bearer ")) {
            security.Bearer.value = "Bearer " + security.Bearer.value;
        }
        originalAuthorize(security);
    };
};