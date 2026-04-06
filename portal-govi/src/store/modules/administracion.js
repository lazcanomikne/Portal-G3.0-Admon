import { axiosInstance } from "../../main";

const Administracion = {
    namespaced: true,
    state: () => ({
        // Puedes agregar estado aquí si es necesario (p.ej. caché de usuarios)
    }),
    actions: {
        getUsers() {
            return axiosInstance.get("/api/Administration/users").then(res => res.data);
        },
        createUser({ dispatch }, user) {
            return axiosInstance.post("/api/Administration/users", user);
        },
        testSap({ commit }, user) {
            return axiosInstance.post("/api/Administration/test-sap", user);
        },
        getPermissionsTree({ commit }, username) {
            return axiosInstance.get(`/api/Administration/permissions/${username}`).then(res => res.data);
        },
        savePermissions({ commit }, payload) {
            return axiosInstance.post("/api/Administration/permissions", payload);
        }
    }
};

export default Administracion;
