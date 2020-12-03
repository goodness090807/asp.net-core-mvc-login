$(document).ready(function () {
    $('#UserLogin').on('click', function () {
        if ($('#LoginForm').valid()) {
            $('#LoginForm').submit();
        }
    })
});