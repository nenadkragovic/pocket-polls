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

export const compare = (a, b) => {
    if ( a.total > b.total ){
      return -1;
    }
    if ( a.total < b.total ){
      return 1;
    }
    return 0;
}