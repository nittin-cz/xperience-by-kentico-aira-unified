import ChatComponent from "./Chat.vue";
import { createApp } from "vue";

const chatElement = document.getElementById("chat-app");

if (chatElement) {
    const pathsModel = JSON.parse(chatElement.dataset.pathsModel || "{}");
    const baseUrl = chatElement.dataset.baseUrl || "";
    const navBarModel = JSON.parse(chatElement.dataset.navBarModel || "{}");
    const history = JSON.parse(chatElement.dataset.history || {});

    createApp(ChatComponent, {
        pathsModel,
        baseUrl,
        navBarModel,
        history
    }).mount("#chat-app");
}