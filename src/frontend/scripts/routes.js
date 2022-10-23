var baseAPI = '';
export var API = {};
export const routesConfig = {
	globalDenominations: '/globalDenominations',
	slotGamesSettings: '/slotGamesSettings',
	rouletteSettings: '/rouletteSettings',
	jackpotSettings: '/jackpotSettings',
};
export const setBaseApi = data => {
	baseAPI = data;
	for (let route in API) {
		API[route] = data + API[route];
	}
};
