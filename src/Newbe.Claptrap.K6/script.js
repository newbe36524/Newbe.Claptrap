import http from "k6/http";
import { check, sleep } from "k6";

export let options = {
    // vus: 3000,
    // duration: "5m",
    stages: [
        { duration: "10s", target: 1000 },
        { duration: "1m", target: 1000 },
    ],
    insecureSkipTLSVerify: true,
};
export default function () {
    const res = http.get("https://localhost:5001/Event");
    // const res = http.get("http://localhost:50001/v1.0/invoke/newbe_claptrap_demo_server/method/Event");
    
    // check(res, {
    //     "protocol is HTTP/2": (r) => r.proto === "HTTP/2.0",
    // });
    // sleep(1);
}
