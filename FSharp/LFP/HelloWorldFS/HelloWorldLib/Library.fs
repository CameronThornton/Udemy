module HelloWorldLibrary

open Newtonsoft.Json

let converToJson str = JsonConvert.SerializeObject str
