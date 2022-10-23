//ENUMS
export const loadActionPages = [{ pageNumber: 4, pageName: 'Crypto Monitoring', strategyType: 'Crypto' }];
//FUNCTIONS
export const getUnique = array => {
	let uniqueArray = [];

	// Loop through array values
	for (let i = 0; i < array.length; i++) {
		if (uniqueArray.indexOf(array[i]) === -1) {
			uniqueArray.push(array[i]);
		}
	}
	return uniqueArray;
};
