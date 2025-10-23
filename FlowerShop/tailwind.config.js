/** @type {import('tailwindcss').Config} */
export default {
    content: [
        './Views/**/*.cshtml',

        './Areas/**/Views/**/*.cshtml',

        './Views/Shared/**/*.cshtml',

        './wwwroot/**/*.html',

        './wwwroot/js/**/*.js',
        
    ],
    theme: {
        extend: {},
    },
    plugins: [],
}
