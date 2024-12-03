import { createApp } from 'vue';
import Chat from './Chat.vue';

const app = createApp({});
app.component('chat-component', Chat);

app.mount('#app');