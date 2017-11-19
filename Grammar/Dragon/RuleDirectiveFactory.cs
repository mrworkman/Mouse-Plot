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
using System.Linq;

namespace Renfrew.Grammar.Dragon {
   using Elements;

   public class RuleDirectiveFactory {

      private Dictionary<Type, ElementGroupings> _groupingTypes;
      private Dictionary<Type, DirectiveTypes> _elementTypes;

      #region Default Constructor
      public RuleDirectiveFactory() : this(
         new Dictionary<Type, ElementGroupings> {
            { typeof(IAlternatives), ElementGroupings.SRCFGO_ALTERNATIVE },
            { typeof(IOptionals),    ElementGroupings.SRCFGO_OPTIONAL    },
            { typeof(IRepeats),      ElementGroupings.SRCFGO_REPEAT      },
            { typeof(ISequence),     ElementGroupings.SRCFGO_SEQUENCE    },
         },
         new Dictionary<Type, DirectiveTypes> {
            { typeof(IListElement),     DirectiveTypes.SRCFG_LIST     },
            { typeof(IRuleElement),     DirectiveTypes.SRCFG_RULE     },
         /* { typeof(IWildcardElement), DirectiveTypes.SRCFG_WILDCARD }, */ // <-- Not Implemented!
            { typeof(IWordElement),     DirectiveTypes.SRCFG_WORD     },
         }) {
      }
      #endregion

      public RuleDirectiveFactory(
         Dictionary<Type, ElementGroupings> groupingTypes,
         Dictionary<Type, DirectiveTypes> elementTypes
      ) {
         _groupingTypes = groupingTypes;
         _elementTypes = elementTypes;
      }

      public RuleDirective CreateEndDirective(IElementContainer grouping) =>
         CreateGroupingDirective(grouping as dynamic, DirectiveTypes.SRCFG_ENDOPERATION);

      public RuleDirective CreateStartDirective(IElementContainer grouping) =>
         CreateGroupingDirective(grouping as dynamic, DirectiveTypes.SRCFG_STARTOPERATION);

      public RuleDirective CreateElementDirective(IElement element, UInt32 id = 0) {
         return new RuleDirective {
            DirectiveType = GetDirectiveType(element),
            ElementGrouping = ElementGroupings.NOT_APPLICABLE,
            Id = id,
         };
      }

      private RuleDirective CreateGroupingDirective(IElementContainer grouping, DirectiveTypes directiveType) {
         return new RuleDirective {
            DirectiveType = directiveType,
            ElementGrouping = GetGroupingType(grouping),
         };
      }

      private ElementGroupings GetGroupingType(IElementContainer grouping) =>
         _groupingTypes.First(t => t.Key.IsAssignableFrom(grouping.GetType())).Value;

      private DirectiveTypes GetDirectiveType(IElement element) =>
         _elementTypes.First(t => t.Key.IsAssignableFrom(element.GetType())).Value;
   }
}
