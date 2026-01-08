// NeoKolors
// Copyright (c) 2025 KryKom

using System.Text;

namespace NeoKolors.Tui;

public class DimensionExpression {

    private readonly List<Dimension> _operands = [];

    public DimensionExpression(Dimension left, Dimension right) {
        _operands.Add(left);
        _operands.Add(right);
    }
    
    public DimensionExpression() { }
    
    public void AddOperand(Dimension operand) => _operands.Add(operand);
    
    public int ToScalar(int parent) {
        int result = 0;
        
        foreach (var d in _operands) {
            result += d.ToScalar(parent);
        }
        
        return result;
    }

    public override string ToString() {
        var sb = new StringBuilder();

        foreach (var o in _operands) {
            sb.Append(o.ToString());
        }
        
        return sb.ToString();
    }
}