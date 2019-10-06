(* Copyright 2019 Yurii Litvinov
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License. *)

namespace Repo

/// Helper functions meant to be used around repository, which implement some widely used non-specific patterns.
module Helpers =
    /// Returns exactly one element of a collection which matches given predicate, throws exceptions produced by
    /// given factory functions if there is no such element or there are more than one.
    let getExactlyOne seq predicate (ifNoneExceptionFactory: unit -> exn) (ifManyExceptionFactory: unit -> exn) =
        let filtered = seq |> Seq.filter predicate

        if Seq.isEmpty filtered then
            raise <| ifNoneExceptionFactory ()
        elif Seq.length filtered <> 1 then
            raise <| ifManyExceptionFactory ()
        else
            Seq.head filtered

    /// Checks that given sequence contains exactly one element and returns it. Throws appropriate exceptions if not.
    let exactlyOneElement elementName seq =
        if Seq.isEmpty seq then
            raise <| ElementNotFoundException elementName
        elif Seq.length seq <> 1 then
            raise <| MultipleElementsException elementName
        else 
            Seq.head seq

    /// Checks that given sequence contains exactly one model and returns it. Throws appropriate exceptions if not.
    let exactlyOneModel modelName seq =
        if Seq.isEmpty seq then
            raise <| ModelNotFoundException modelName
        elif Seq.length seq <> 1 then
            raise <| MultipleModelsException modelName
        else 
            Seq.head seq
