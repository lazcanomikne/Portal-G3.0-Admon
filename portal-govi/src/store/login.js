import { axiosInstance } from "../main";

const LoginModule = {
  namespaced: true,
  state: () => ({
    isLogin: null,
    userName: "",
    userPass: "",
  }),
  mutations: {
    SET_LOGIN: (state, datos) => {
      state.isLogin = datos;
    },
    SET_USERNAME: (state, datos) => {
      state.userName = datos;
    },
    SET_USERPASS: (state, datos) => {
      state.userPass = datos;
    },
  },
  actions: {
    login: ({ commit }, data) => {
      return new Promise((resolve, reject) => {
        axiosInstance
          .post(`/api/dataapp/login`, data)
          .then((res) => {
            commit("SET_LOGIN", true);
            localStorage.setItem("b1session", res.headers["b1session"]);
            localStorage.setItem("routeid", res.headers["routeid"]);
            commit("SET_USERNAME", data.UserName);
            commit("SET_USERPASS", data.Password);
            resolve(true);
          })
          .catch((err) => {
            reject(err.response);
          });
      });
    },
    loginSap: async ({ commit }, data) => {
      try {
        const req = await axiosInstance.post(
          "https://192.168.1.30:50000/b1s/v1/Login",
          {
            CompanyDB: "SBODEMOGOVI2020",
            UserName: data.UserName,
            Password: data.Password,
          },
          { withCredentials: true }
        );
        const res = await req.data;

        console.log(res);
        return true;
      } catch (error) {
        console.error(error);
        return false;
      }
    },
  },
};

export default LoginModule;
