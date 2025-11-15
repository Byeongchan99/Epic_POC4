# 빠른 시작 가이드 ⚡

## 1️⃣ GameManager 생성 (1분)

1. **Hierarchy 우클릭** → `Create Empty`
2. 이름: `GameManager`
3. **Add Component** → `GameManager` 검색 후 추가

## 2️⃣ SlashDetector 생성 (30초)

1. **Hierarchy 우클릭** → `Create Empty`
2. 이름: `SlashDetector`
3. **Add Component** → `SlashDetector` 검색 후 추가

## 3️⃣ UI 생성 (2분)

1. **Hierarchy 우클릭** → `UI > Canvas`

2. **Canvas 우클릭** → `UI > Text` (4번 반복)
   - 이름 변경: `StatusText`, `TimerText`, `ScoreText`, `InstructionText`

### StatusText 설정:
- Text: `Stage 1 - Cut 0/3 lines!`
- Font Size: `32`
- Rect Transform > Anchor: **Center**
- Pos Y: `200`

### TimerText 설정:
- Text: `Time: 3.0s`
- Font Size: `48`
- Color: **빨간색**
- Rect Transform > Anchor: **Top Center**
- Pos Y: `-50`

### ScoreText 설정:
- Text: `Score: 0`
- Font Size: `24`
- Rect Transform > Anchor: **Top Left**
- Pos X: `150`, Pos Y: `-25`

### InstructionText 설정:
- Text: `Click and drag to slash through magic circles!`
- Font Size: `20`
- Color: **회색**
- Rect Transform > Anchor: **Bottom Center**
- Pos Y: `30`

## 4️⃣ GameManager 연결 (1분)

**GameManager** 선택 → Inspector:

### References:
- **Magic Circle Prefab**: `Assets/Prefabs/MagicCircle` 드래그
- **Slash Detector**: Hierarchy의 `SlashDetector` 드래그
- **Status Text**: `Canvas > StatusText` 드래그
- **Timer Text**: `Canvas > TimerText` 드래그
- **Score Text**: `Canvas > ScoreText` 드래그

### Game Settings (기본값 그대로 OK):
- Cast Time: `3`
- Cuts Needed To Win: `3`
- Spawn Position: `(0, 0)`

## 5️⃣ 완료! ✅

**Play 버튼 클릭** → 게임 시작!

---

## 🎨 값 조정하기

### 난이도 변경
**GameManager** Inspector:
- `Cast Time`: **2** (어려움) / **5** (쉬움)
- `Cuts Needed To Win`: **5** (어려움) / **2** (쉬움)

### 마법진 색상 변경
**Project > Assets/Prefabs/MagicCircle** 선택:
- `Circle Color`: 원하는 색으로 변경
- `Line Width`: `0.15` (두꺼움) / `0.05` (얇음)

### 검기 색상 변경
**SlashDetector** Inspector:
- `Slash Color`: 원하는 색으로 변경
- `Slash Width`: `0.2` (두꺼움) / `0.1` (얇음)

---

**총 소요 시간: 약 5분** 🚀
