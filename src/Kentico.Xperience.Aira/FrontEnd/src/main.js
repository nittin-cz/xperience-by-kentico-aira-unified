import ChatComponent from "./Chat.vue";
import { createApp } from "vue";

const chatElement = document.getElementById("chat-app");

if (chatElement) {
    const pathsModel = JSON.parse(chatElement.dataset.pathsModel || "{}");
    const baseUrl = chatElement.dataset.baseUrl || "";

    createApp(ChatComponent, {
        pathsModel,
        baseUrl,
    }).mount("#chat-app");
}