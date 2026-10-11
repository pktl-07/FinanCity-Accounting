export function getHeaderScheme(expression?: {}) {

    let headerJson = {
        'authorize': 'authorize'
    };

    if (typeof expression != "undefined" || expression != null) {
        Object.getOwnPropertyNames(expression).forEach(f => {
            if (expression[f] !== undefined) {

                headerJson[f.toString()] = expression[f];
            }
        });
    }
    return headerJson;
}







