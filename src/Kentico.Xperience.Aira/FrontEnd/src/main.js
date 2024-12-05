import { createApp } from "vue";

// A basic Vue component (optional)
const App = {
    template: "<div>Vue is working! This is {{ message }}</div>",
    data() {
        return {
            message: "a simple Vue app",
        };
    },
};

// Mount Vue to a specific DOM element
const app = createApp(App);
app.mount("#vue-app");