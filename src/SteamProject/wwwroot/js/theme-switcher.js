document.addEventListener('DOMContentLoaded', function () {
    // Apply the user's theme preference on page load
    applyThemePreference();

    $("#hide-checkbox").on("change", function () {
        var theme = $(this).prop("checked") ? "light" : "dark";
        updateTheme(theme);
    });
});

// Function to apply the user's theme preference
function applyThemePreference() {
    var theme = $("#currentTheme").val();
    if (theme) {
        var themeLink = $("#theme-link");
        themeLink.attr("href", "/css/" + theme + "-theme.css");

        // Set the initial state of the toggle switch
        if (theme === "dark") {
            $("#hide-checkbox").prop("checked", false);
        } else {
            $("#hide-checkbox").prop("checked", true);
        }
    }
}



// Function to update the theme
function updateTheme(theme) {
    var url = `/api/Steam/UpdateTheme?theme=${theme}`;

    console.log("Updating theme to:", theme); // Log the theme
    console.log("API URL:", url); // Log the API URL

    $.ajax({
        type: "POST",
        dataType: "text",
        url: url,
        success: function () {
            // Update the theme on the client side
            var themeLink = $("#theme-link");
            themeLink.attr("href", "/css/" + theme + "-theme.css");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error("Error updating theme: ", jqXHR);
        },
    });
}

