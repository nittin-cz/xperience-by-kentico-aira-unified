<template>
    <div class="c-app_inner" v-bind:class="{'is-loaded':isLoaded}">
        <div id="loading" class="c-loading">
            <img :src="`${this.baseUrl}/_content/Kentico.Xperience.AiraUnified/img/spinner.svg`" alt="loading spinner"
                 class="c-loading_spinner"/>
        </div>

        <div class="c-app_header">
            <NavBarComponent :airaUnifiedBaseUrl="airaUnifiedBaseUrl" :navigationPageIdentifier="navigationPageIdentifier" :navigationUrl="navigationUrl" :baseUrl="baseUrl"/>
        </div>
        <div class="c-app_body" :style="{ maxHeight: '60vh', overflowY: 'auto', overflowX: 'hidden' }">
            <div>
                <a :href="`${chatUrl}?${chatQueryParameterName}=${thread.threadId}`" class="c-thread" v-for="thread in threadsData">
                    <span class="d-block c-thread_icon">
                        <svg xmlns="http://www.w3.org/2000/svg" width="66" height="65" viewBox="0 0 66 65" fill="none">
                            <path fill-rule="evenodd" clip-rule="evenodd" d="M55.7381 10.6021L64.5916 13.5533C65.8028 13.9571 65.8028 15.6703 64.5916 16.074L55.7381 19.0252C55.3414 19.1574 55.0301 19.4687 54.8978 19.8654L51.9467 28.7189C51.5429 29.9301 49.8297 29.9301 49.426 28.7189L46.4748 19.8654C46.3426 19.4687 46.0313 19.1574 45.6346 19.0252L36.7811 16.074C35.5699 15.6703 35.5699 13.9571 36.7811 13.5533L45.6346 10.6021C46.0313 10.4699 46.3426 10.1586 46.4748 9.76191L49.426 0.908415C49.8297 -0.302802 51.5429 -0.302806 51.9467 0.908411L54.8978 9.76191C55.0301 10.1586 55.3414 10.4699 55.7381 10.6021ZM54.6178 13.9631L57.1695 14.8137L54.6178 15.6642C53.1632 16.1491 52.0218 17.2905 51.5369 18.7451L50.6863 21.2968L49.8358 18.7451C49.3509 17.2905 48.2095 16.1491 46.7549 15.6642L44.2032 14.8137L46.7549 13.9631C48.2095 13.4782 49.3509 12.3368 49.8358 10.8822L50.6863 8.3305L51.5369 10.8822C52.0218 12.3368 53.1632 13.4782 54.6178 13.9631Z" fill="black"/>
                            <path fill-rule="evenodd" clip-rule="evenodd" d="M25.8924 27.1831C25.4957 27.0508 25.1844 26.7395 25.0522 26.3428L20.9956 14.1732C20.5919 12.9619 18.8786 12.9619 18.4749 14.1732L14.4183 26.3428C14.2861 26.7395 13.9748 27.0508 13.5781 27.1831L1.40841 31.2396C0.197197 31.6434 0.197194 33.3566 1.40841 33.7604L13.5781 37.8169C13.9748 37.9492 14.2861 38.2605 14.4183 38.6572L18.4749 50.8268C18.8786 52.0381 20.5919 52.0381 20.9956 50.8269L25.0522 38.6572C25.1844 38.2605 25.4957 37.9492 25.8924 37.8169L38.0621 33.7604C39.2733 33.3566 39.2733 31.6434 38.0621 31.2396L25.8924 27.1831ZM30.64 32.5L24.7721 30.544C23.3175 30.0592 22.1761 28.9177 21.6912 27.4632L19.7353 21.5952L17.7793 27.4632C17.2944 28.9177 16.153 30.0592 14.6984 30.544L8.83051 32.5L14.6984 34.456C16.153 34.9408 17.2944 36.0822 17.7793 37.5368L19.7353 43.4048L21.6912 37.5368C22.1761 36.0822 23.3175 34.9408 24.7721 34.456L30.64 32.5Z" fill="black"/>
                            <path d="M37.8968 51.5018C38.2935 51.3695 38.6048 51.0583 38.737 50.6615L40.5828 45.1242C40.9865 43.913 42.6998 43.913 43.1035 45.1242L44.9493 50.6615C45.0815 51.0583 45.3928 51.3695 45.7895 51.5018L51.3268 53.3476C52.5381 53.7513 52.5381 55.4645 51.3268 55.8683L45.7895 57.714C45.3928 57.8463 45.0815 58.1576 44.9493 58.5543L43.1035 64.0916C42.6998 65.3028 40.9865 65.3028 40.5828 64.0916L38.737 58.5543C38.6048 58.1576 38.2935 57.8463 37.8968 57.714L32.3595 55.8683C31.1483 55.4645 31.1483 53.7513 32.3595 53.3476L37.8968 51.5018Z" fill="black"/>
                        </svg>
                    </span>
                        <span class="c-thread_body">
                        <span class="c-thread_header">
                            <span class="c-thread_title">{{thread.threadName}}</span>
                            <span class="c-thread_indicator"></span>
                            <span class="c-thread_time">{{thread.lastUsed}}</span>
                        </span>
                        <span class="c-thread_content">
                            {{thread.lastMessage}}
                        </span>
                    </span>
                </a>
                <div class="d-flex justify-content-end mt-2">
                    <a :href="newChatThreadUrl" class="btn btn-link secondary">
                        <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" viewBox="0 0 13 13" fill="none" class="c-icon">
                            <path d="M2.99731 6.50098C2.99731 6.22483 3.22117 6.00098 3.49731 6.00098H5.99243V3.50195C5.99243 3.22581 6.21629 3.00195 6.49243 3.00195C6.76857 3.00195 6.99243 3.22581 6.99243 3.50195V6.00098H9.49731C9.77346 6.00098 9.99731 6.22483 9.99731 6.50098C9.99731 6.77712 9.77346 7.00098 9.49731 7.00098H6.99243V9.50195C6.99243 9.7781 6.76857 10.002 6.49243 10.002C6.21629 10.002 5.99243 9.7781 5.99243 9.50195V7.00098H3.49731C3.22117 7.00098 2.99731 6.77712 2.99731 6.50098Z" fill="currentColor"/>
                            <path fill-rule="evenodd" clip-rule="evenodd" d="M0 1.5039C0 0.675477 0.671574 0.00390625 1.5 0.00390625H11.4965C12.325 0.00390625 12.9965 0.67548 12.9965 1.50391V11.5004C12.9965 12.3289 12.325 13.0004 11.4965 13.0004H1.5C0.671571 13.0004 0 12.3289 0 11.5004V1.5039ZM1.5 1.00391C1.22386 1.00391 1 1.22776 1 1.5039V11.5004C1 11.7766 1.22386 12.0004 1.5 12.0004H11.4965C11.7727 12.0004 11.9965 11.7766 11.9965 11.5004V1.50391C11.9965 1.22776 11.7727 1.00391 11.4965 1.00391H1.5Z" fill="currentColor"/>
                        </svg>
                        NEW CHAT
                    </a>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import NavBarComponent from "./Navigation.vue";

export default {
    components: {
        NavBarComponent
    },
    props: {
        airaUnifiedBaseUrl: null,
        baseUrl: null,
        navigationUrl: null,
        navigationPageIdentifier: null,
        userThreadCollectionUrl: null,
        chatUrl: null,
        chatQueryParameterName: null,
        newChatThreadUrl: null
    },
    data() {
        return {
            threadsData: []
        }
    },
    mounted() {
        document.onreadystatechange = () => {
            if (document.readyState === "complete") {
                this.main();
            }
        }
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

            this.getUserThreadCollection();
        },
        async getUserThreadCollection() {
            const threadResponse = await fetch(this.userThreadCollectionUrl, {
                method: 'GET'
            });

            if (!threadResponse.ok)
            {
                console.error('An error occurred:', threadResponse.error);
                return;
            }
            const data = await threadResponse.json();

            this.threadsData = data.chatThreads;
        }
    }
};

</script>