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
    }, safelist: [
        'bg-emerald-900/90',
        'bg-amber-900/90',
        'bg-red-900/90',
        'bg-gray-900/90',
        'bg-blue-900/90',
        'text-blue-400',
        'text-red-400',
        'text-amber-400',
        'text-emerald-400',
        'text-gray-400'
    ],
    plugins: [],
}
