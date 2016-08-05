// Project Renfrew
// Copyright(C) 2016  Stephen Workman (workman.stephen@gmail.com)
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.If not, see<http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Renfrew.Grammar.FluentApi {
   using Elements;
   
   public interface IRule {
      IElementContainer Elements { get; }

      IActionableRule Say(String word);
      IActionableRule SayOneOf(IEnumerable<String> words);
      IActionableRule SayOneOf(params String[] words);

      IActionableRule OneOf(params Expression<Action<IRule>>[] actions);

      IActionableRule Optionally(Expression<Action<IRule>> action);
      IActionableRule OptionallyOneOf(params Expression<Action<IRule>>[] actions);

      IActionableRule OptionallySay(String word);

      IActionableRule Repeat(Expression<Action<IRule>> action);
      IActionableRule RepeatOneOf(params Expression<Action<IRule>>[] actions);
   }
}
