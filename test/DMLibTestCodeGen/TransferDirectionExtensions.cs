﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMLibTestCodeGen
{
    static class TransferDirectionExtensions
    {
        public static IEnumerable<CodeStatement> EnumerateUpdateContextStatements(DMLibTransferDirection transferDirection)
        {
            CodeFieldReferenceExpression sourceType = new CodeFieldReferenceExpression(
                new CodeTypeReferenceExpression(typeof(DMLibDataType)),
                transferDirection.SourceType.ToString());
            CodeFieldReferenceExpression destType = new CodeFieldReferenceExpression(
                new CodeTypeReferenceExpression(typeof(DMLibDataType)),
                transferDirection.DestType.ToString());

            CodePropertyReferenceExpression sourceTypeProperty = new CodePropertyReferenceExpression(
                new CodeTypeReferenceExpression(typeof(DMLibTestContext)),
                "SourceType");

            CodePropertyReferenceExpression destTypeProperty = new CodePropertyReferenceExpression(
                new CodeTypeReferenceExpression(typeof(DMLibTestContext)),
                "DestType");

            CodePropertyReferenceExpression isAsyncProperty = new CodePropertyReferenceExpression(
                new CodeTypeReferenceExpression(typeof(DMLibTestContext)),
                "IsAsync");

            yield return new CodeAssignStatement(sourceTypeProperty, sourceType);

            yield return new CodeAssignStatement(destTypeProperty, destType);

            yield return new CodeAssignStatement(isAsyncProperty, new CodePrimitiveExpression(transferDirection.IsAsync));
        }
    }
}
