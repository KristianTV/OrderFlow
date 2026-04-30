// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function createElement(tag, properties, container) {
    const element = document.createElement(tag);

    Object.keys(properties).forEach((key) => {
        if (typeof properties[key] === 'object') {
            element[key] ??= {};
            Object.assign(element[key], properties[key])
        } else {
            element[key] = properties[key]
        }
    });

    if (container) container.append(element);

    return element;
}