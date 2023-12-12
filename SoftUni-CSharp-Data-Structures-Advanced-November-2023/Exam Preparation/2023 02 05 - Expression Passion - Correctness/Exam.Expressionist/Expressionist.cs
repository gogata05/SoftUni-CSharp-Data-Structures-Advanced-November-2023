using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.Expressionist
{
    public class Expressionist : IExpressionist
    {
        private Dictionary<string, Expression> expressions = new Dictionary<string, Expression>();

        public void AddExpression(Expression expression)
        {
            if (expressions.Count > 0) throw new ArgumentException();
            expressions[expression.Id] = expression;
        }

        public void AddExpression(Expression expression, string parentId)
        {
            if (!expressions.ContainsKey(parentId)) throw new ArgumentException();

            var parentExpression = expressions[parentId];
            if (parentExpression.LeftChild == null) parentExpression.LeftChild = expression;
            else if (parentExpression.RightChild == null) parentExpression.RightChild = expression;
            else throw new ArgumentException();

            expressions[expression.Id] = expression;
        }

        public bool Contains(Expression expression)
        {
            return expressions.ContainsKey(expression.Id);
        }

        public int Count()
        {
            return expressions.Count;
        }

        public string Evaluate()
        {
            if (expressions.Count == 0) return string.Empty;
            return Evaluate(expressions.Values.FirstOrDefault(e => e.LeftChild != null || e.RightChild != null));
        }

        private string Evaluate(Expression expression)
        {
            if (expression == null) return "";
            if (expression.Type == ExpressionType.Value) return expression.Value;
            return $"({Evaluate(expression.LeftChild)} {expression.Value} {Evaluate(expression.RightChild)})";
        }

        public Expression GetExpression(string expressionId)
        {
            if (!expressions.ContainsKey(expressionId)) throw new ArgumentException();
            return expressions[expressionId];
        }

        public void RemoveExpression(string expressionId)
        {
            if (!expressions.ContainsKey(expressionId)) throw new ArgumentException();
            RemoveExpressionRecursively(expressionId);
        }

        private void RemoveExpressionRecursively(string expressionId)
        {
            var expression = expressions[expressionId];
            expressions.Remove(expressionId);

            if (expression.LeftChild != null) RemoveExpressionRecursively(expression.LeftChild.Id);
            if (expression.RightChild != null) RemoveExpressionRecursively(expression.RightChild.Id);

            foreach (var exp in expressions.Values)
            {
                if (exp.LeftChild?.Id == expressionId) exp.LeftChild = exp.RightChild;
                if (exp.RightChild?.Id == expressionId) exp.RightChild = null;
            }
        }
    }
}
