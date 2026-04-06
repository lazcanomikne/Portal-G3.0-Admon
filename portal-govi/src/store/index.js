import Vue from "vue";
import Vuex from "vuex";
import createPersistedState from "vuex-persistedstate";

import AjustesModule from "./ajustes";
import Cancelacion from "./cancelacion";
import Config from "./config";
import Credito from "./credito";
import Dispersion from "./dispersion";
import Informes from "./informes";
import Inversion from "./inversion";
import Usuarios from "./usuarios";
import LoginModule from "./login";
import Notas from "./notas";
import TunelBancario from "./tunel";
import ConfigCuentas from "./configCuentas";
import Administracion from "./modules/administracion";

Vue.use(Vuex);

export default new Vuex.Store({
  modules: {
    dispersion: Dispersion,
    informes: Informes,
    login: LoginModule,
    ajustes: AjustesModule,
    tunel: TunelBancario,
    config: Config,
    notas: Notas,
    cancelacion: Cancelacion,
    credito: Credito,
    inversion: Inversion,
    usuarios: Usuarios,
    configCuentas: ConfigCuentas,
    administracion: Administracion,
  },
  plugins: [
    createPersistedState({
      key: "AIzaSyC4d3pkEslVbx3xLc1037FdfgoYPW6b-yI",
      paths: ["login.userName"],
      storage: window.localStorage,
    }),
  ],
});
