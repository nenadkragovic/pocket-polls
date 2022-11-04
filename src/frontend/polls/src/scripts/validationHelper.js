export const getValidationMessage = (response) => {
    try
    {
        if (response.errors != null) {
            var keys = Object.keys(response.errors);
            return response.errors[keys[0]];
        }
    }
    catch (e) {
        return 'Something went wrong!'
    }
}