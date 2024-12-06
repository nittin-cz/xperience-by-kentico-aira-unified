<template>
    <div class="c-app_inner">
        <div id="loading" class="c-loading">
            <img :src="`${baseUrl}/_content/Kentico.Xperience.Aira/img/spinner.svg`" class="c-loading_spinner" alt="loading spinner" />
        </div>

        <div class="c-app_header">
        </div>

        <div class="c-app_body">
            <div class="container">
                <deep-chat 
                    :connect="{
                        url: `${baseUrl}/${pathsModel.PathBase}/${pathsModel.ChatMessagePath}`,
                        method: 'POST'
                    }"
                    :chatStyle="{ height: '100%', width: '100%' }"
                    id="chatElement"
                    ref="chatElementRef"
                    :requestBodyLimits="{ maxMessages: 1 }"
                    style="border-radius: 8px;"
                    :introMessage="{ text: '' }"
                    :messageStyles="{
                        default: {
                            shared: { bubble: { fontSize: '0.75rem', lineHeight: '1.375rem', padding: '0.5rem 0.75rem' } },
                            ai: { bubble: { background: '#F7F1FF' } },
                            user: { bubble: { color: '#fff' } }
                        },
                        image: {
                            user: { bubble: { borderRadius: '1rem', overflow: 'clip', textAlign: 'left', display: 'inline-block' } }
                        }
                    }">
                </deep-chat>
            </div>
        </div>
    </div>
</template>

<script>
import 'deep-chat';

export default {
    props: {
        pathsModel: null,
        baseUrl: null
    },
    data() {
        return {
            themeColor: "#8107c1",
            themeColorInRgb: "rgb(129, 7, 193)",
            submitButton: null,
            started: false
        }
    },
    mounted() {
        document.onreadystatechange = () => {
            if (document.readyState === "complete") {
                this.main();
            }
        };
    },
    methods: {
        main() {
            setTimeout(() => {
                var modal = document.querySelector('#loading');
                if (modal) {
                    modal.classList.add('is-hidden');
                }
                setTimeout(function () {
                    modal.parentNode.removeChild(modal);
                }, 500);
            }, 1000);

            if (!this.started) {
                this.started = true;

                document.addEventListener('visibilitychange', function () {
                    if (document.visibilityState === 'visible') {
                        app.$refs.chatElementRef.scrollToBottom();
                    }
                });

                this.$refs.chatElementRef.onComponentRender = async () => {
                    this.$refs.chatElementRef.style.borderLeftStyle = 'none';
                    this.$refs.chatElementRef.style.borderTopStyle = 'none';
                    this.$refs.chatElementRef.style.borderRightStyle = 'none';
                    this.$refs.chatElementRef.style.borderBottomStyle = 'none';
                    console.log(this.$refs.chatElementRef.shadowRoot);
                    const newSubmitButton = this.$refs.chatElementRef.shadowRoot.querySelector('.input-button');
                    if (this.submitButton !== newSubmitButton) {
                        this.submitButton = newSubmitButton;
                        this.addClassesToShadowRoot();
                    }
                };
            }
        },
        enableSubmitButtonForUpload() {
            //this.submitButton.addEventListener('click', this.customSubmitFunction);
            //this.submitButton.addEventListener('mouseenter', this.enableOnHover);
        },
        addClassesToShadowRoot() {
            let shadowRoot = this.$refs.chatElementRef.shadowRoot;

            const style = document.createElement('style');

            style.textContent =
                `.btn-outline-primary {
                    color: ${this.themeColorInRgb};
                    background-color: transparent;
                    border: 1px solid ${this.themeColorInRgb};
                    font-size: 0.75rem;
                    border-radius: 24px;
                    padding: 6px 12px;
                    margin: 4px;
                    transition: color 0.3s ease, background-color 0.3s ease, border-color 0.3s ease;
                    display: inline-block;
                }

                .user-message-text {
                    background-color: ${this.themeColor} !important;
                }

                .btn-outline-primary:hover {
                    color: #fff;
                    background-color: ${this.themeColorInRgb};
                    border-color: ${this.themeColorInRgb};
                }

                .btn-outline-primary:active {
                    color: #fff;
                    background-color: #C64300;
                    border-color: #C64300;
                }

                .btn-outline-primary:disabled {
                        color: ${this.themeColorInRgb};
                    background-color: transparent;
                        border-color: ${this.themeColorInRgb};
                    opacity: 0.65;
                    pointer-events: none;
                }

                .container {
                    display: flex;
                    flex-direction: row;
                    align-items: flex-start;
                    flex-wrap: wrap;
                }

                .message-bubble div {
                    margin-bottom: 8px;
                }

                .message-bubble .btn-outline-primary {
                    margin-right: 8px;
                }

                .lds-ring, .lds-ring div {
                    box-sizing: border-box;
                }

                .lds-ring {
                    display: inline-block;
                    position: relative;
                    width: 80px;
                    height: 80px;
                    margin-bottom: 0px;
                }

                .lds-ring div {
                    box-sizing: border-box;
                    display: block;
                    position: absolute;
                    width: 64px;
                    height: 64px;
                    margin: 8px;
                    border: 5px solid currentColor;
                    border-radius: 50%;
                    animation: lds-ring 1.2s cubic-bezier(0.5, 0, 0.5, 1) infinite;
                    border-color: currentColor transparent transparent transparent;
                }

                .lds-ring div:nth-child(1) {
                    animation-delay: -0.45s;
                }

                .lds-ring div:nth-child(2) {
                    animation-delay: -0.3s;
                }

                .lds-ring div:nth-child(3) {
                    animation-delay: -0.15s;
                }

                @@keyframes lds-ring {
                    0% {
                        transform: rotate(0deg);
                    }

                    100% {
                        transform: rotate(360deg);
                    }
                }`
            shadowRoot.appendChild(style);
        },
    }
};
</script>