/** @type {import('tailwindcss').Config} */
export default {
    content: [
        // Glavni Views folder
        './Views/**/*.cshtml',

        // Views u Areas (ako ih koristiš)
        './Areas/**/Views/**/*.cshtml',

        // Layout fajlovi (ako ih imaš u Shared)
        './Views/Shared/**/*.cshtml',

        // Static HTML fajlovi ako ih imaš (npr. landing page)
        './wwwroot/**/*.html',

        // JS fajlovi (ako koristiš Tailwind klase unutar JS-a)
        './wwwroot/js/**/*.js',
    ],
    theme: {
        extend: {},
    },
    plugins: [],
}
