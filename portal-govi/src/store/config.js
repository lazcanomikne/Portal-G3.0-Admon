import { axiosInstance } from "../main";

const Config = {
  namespaced: true,
  state: () => ({
    menus: [],
    canCreate: true,
  }),
  getters: {
    doneMenu: (state) => state.menus,
    doneCanCreate: (state) => state.canCreate,
  },
  mutations: {
    SET_MENUS: (state, datos) => {
      state.menus = datos;
    },
    SET_CANCREATE: (state, datos) => {
      state.canCreate = datos;
    },
  },
  actions: {
    getMenu: ({ commit, rootState }) => {
      let user = rootState.login.userName;
      axiosInstance.get(`/api/config/menu?u=${user}`).then((res) => {
        commit("SET_MENUS", res.data);
      });
    },
  },
};

export default Config;
