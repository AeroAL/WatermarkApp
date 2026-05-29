#!/bin/bash

echo "========================================"
echo "GitHub Actions 自动编译配置脚本"
echo "========================================"
echo ""

# 检查是否安装了 git
if ! command -v git &> /dev/null; then
    echo "错误：未安装 git"
    echo "请运行: sudo apt install git"
    exit 1
fi

# 检查是否安装了 gh (GitHub CLI)
if ! command -v gh &> /dev/null; then
    echo "提示：未安装 GitHub CLI"
    echo "如需自动创建仓库，请运行: sudo apt install gh"
    echo "或者手动在 GitHub 网站创建仓库"
    echo ""
fi

# 初始化 Git 仓库
if [ ! -d ".git" ]; then
    echo "正在初始化 Git 仓库..."
    git init
    echo "Git 仓库初始化完成"
else
    echo "Git 仓库已存在"
fi

# 创建 .gitignore
echo "正在创建 .gitignore..."
cat > .gitignore << 'EOF'
# Build results
bin/
obj/
[Dd]ebug/
[Rr]elease/
*.exe
*.dll
*.pdb

# Visual Studio
.vs/
*.suo
*.user
*.userosscache
*.sln.docstates

# Inno Setup
Output/

# OS files
.DS_Store
Thumbs.db
EOF

echo ".gitignore 创建完成"
echo ""

# 添加所有文件
echo "正在添加文件到 Git..."
git add .
echo ""

# 显示状态
echo "当前 Git 状态："
git status
echo ""

echo "========================================"
echo "下一步操作："
echo "========================================"
echo ""
echo "1. 提交代码："
echo "   git commit -m \"Initial commit\""
echo ""
echo "2. 在 GitHub 创建新仓库："
echo "   - 访问 https://github.com/new"
echo "   - 仓库名：WatermarkApp"
echo "   - 选择 Public 或 Private"
echo ""
echo "3. 推送到 GitHub："
echo "   git remote add origin https://github.com/你的用户名/WatermarkApp.git"
echo "   git branch -M main"
echo "   git push -u origin main"
echo ""
echo "4. 等待自动编译："
echo "   - 推送后 GitHub Actions 会自动运行"
echo "   - 在仓库的 Actions 标签页查看编译进度"
echo "   - 编译完成后在 Artifacts 下载安装包"
echo ""
echo "========================================"
